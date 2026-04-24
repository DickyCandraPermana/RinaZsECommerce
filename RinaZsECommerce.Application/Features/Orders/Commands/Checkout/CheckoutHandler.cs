using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Orders.Commands.Checkout;

public class CheckoutHandler : IRequestHandler<CheckoutCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) throw new Exception("User not found");

        var pagedCartItems = await _unitOfWork.Carts.GetPaginatedAsync(new CartItemFilter { UserId = user.ProfileId });
        var cartItems = pagedCartItems.Items.ToList();
        
        if (!cartItems.Any()) throw new Exception("Cart is empty");

        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = user.ProfileId,
            Description = request.Description,
            Address = request.Address,
            Status = OrderStatus.Pending,
            TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Amount),
            User = user.UserProfile,
            Details = cartItems.Select(c => new OrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = c.ProductId,
                Amount = c.Amount,
                PriceAtPurchase = c.Product?.Price ?? 0,
                Product = null! // Navigation
            }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);

        // Clear cart
        foreach (var item in cartItems)
        {
            _unitOfWork.Carts.Delete(item);
        }

        await _unitOfWork.CompleteAsync();

        return order.Id;
    }
}
