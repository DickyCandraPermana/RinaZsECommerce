using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Admin.Orders.Queries.GetAllOrders;

public class GetAllOrdersAdminHandler : IRequestHandler<GetAllOrdersAdminQuery, IEnumerable<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllOrdersAdminHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<OrderResponse>> Handle(GetAllOrdersAdminQuery request, CancellationToken cancellationToken)
    {
        var paged = await _unitOfWork.Orders.GetPaginatedAsync(request.Filter);
        return paged.Items.Select(o => new OrderResponse(
            o.Id,
            o.UserId,
            $"{o.User?.FirstName} {o.User?.LastName}".Trim(),
            o.User?.User?.Email ?? "N/A",
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
