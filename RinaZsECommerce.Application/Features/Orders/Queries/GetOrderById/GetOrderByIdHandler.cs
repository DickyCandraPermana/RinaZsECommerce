using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return null;

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return null;

        // Validasi kepemilikan kecuali user adalah Admin
        if (order.UserId != user.ProfileId && user.Role != "Admin")
            return null;

        return new OrderResponse(
            order.Id,
            order.UserId,
            $"{order.User?.FirstName} {order.User?.LastName}".Trim(),
            order.User?.User?.Email ?? user.Email,
            order.Address,
            order.Description,
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.Details?.Select(d => new OrderDetailResponse(
                d.Id,
                d.ProductId,
                d.Product?.Name ?? "Item Pesanan",
                d.Product?.Images?.FirstOrDefault()?.ImageUrl,
                d.Amount,
                d.PriceAtPurchase,
                d.Amount * d.PriceAtPurchase
            )).ToList() ?? new List<OrderDetailResponse>()
        );
    }
}
