using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryHandler : IRequestHandler<GetOrderHistoryQuery, IEnumerable<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderHistoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OrderResponse>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return Enumerable.Empty<OrderResponse>();

        var pagedOrders = await _unitOfWork.Orders.GetPaginatedAsync(new OrderFilter 
        { 
            UserId = user.ProfileId,
            PageSize = 100,
            IsAscending = false
        });

        return pagedOrders.Items.Select(o => new OrderResponse(
            o.Id,
            o.UserId,
            $"{user.UserProfile?.FirstName} {user.UserProfile?.LastName}".Trim(),
            user.Email,
            o.Address,
            o.Description,
            o.Status.ToString(),
            o.TotalAmount,
            o.CreatedAt,
            o.Details?.Select(d => new OrderDetailResponse(
                d.Id,
                d.ProductId,
                d.Product?.Name ?? "Item Pesanan",
                d.Product?.Images?.FirstOrDefault()?.ImageUrl,
                d.Amount,
                d.PriceAtPurchase,
                d.Amount * d.PriceAtPurchase
            )).ToList() ?? new List<OrderDetailResponse>()
        ));
    }
}
