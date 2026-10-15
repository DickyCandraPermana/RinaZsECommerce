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
        if (user == null) throw new KeyNotFoundException("User tidak ditemukan.");

        var pagedCartItems = await _unitOfWork.Carts.GetPaginatedAsync(new CartItemFilter { UserId = user.ProfileId });
        var cartItems = pagedCartItems.Items.ToList();
        
        if (!cartItems.Any()) throw new InvalidOperationException("Keranjang belanja masih kosong.");

        // Validasi dan kurangi stok untuk setiap produk
        foreach (var item in cartItems)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Produk dengan ID '{item.ProductId}' tidak ditemukan.");

            if (product.Stock < item.Amount)
                throw new InvalidOperationException($"Stok produk '{product.Name}' tidak mencukupi. Tersisa {product.Stock} unit.");

            // Potong stok dan catat stock log
            await _unitOfWork.Products.UpdateStockAsync(
                product.Id,
                -item.Amount,
                user.ProfileId,
                StockTransactionType.Sale);
        }

        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = user.ProfileId,
            Description = request.Description,
            Address = string.IsNullOrWhiteSpace(request.Address) ? (user.UserProfile?.Address ?? "Alamat belum diatur") : request.Address,
            Status = OrderStatus.Pending,
            TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Amount),
            User = null!,
            Details = cartItems.Select(c => new OrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = c.ProductId,
                Amount = c.Amount,
                PriceAtPurchase = c.Product?.Price ?? 0,
                Product = null!,
                Order = null!
            }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);

        // Bersihkan keranjang
        await _unitOfWork.Carts.ClearCartAsync(user.ProfileId);
        await _unitOfWork.CompleteAsync();

        return order.Id;
    }
}
