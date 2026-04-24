using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;
using RinaZsECommerce.Domain.Entities.Filter;

namespace RinaZsECommerce.Application.Features.Carts.Queries.GetCartItems;

public class GetCartItemsHandler : IRequestHandler<GetCartItemsQuery, IEnumerable<CartItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCartItemsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CartItemDto>> Handle(GetCartItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return Enumerable.Empty<CartItemDto>();

        var pagedCartItems = await _unitOfWork.Carts.GetPaginatedAsync(new CartItemFilter { UserId = user.ProfileId });
        var cartItems = pagedCartItems.Items;
        
        return cartItems.Select(c => new CartItemDto(
            c.Id,
            c.ProductId,
            c.Product?.Name ?? "Unknown Product",
            c.Product?.Price ?? 0,
            c.Amount,
            (c.Product?.Price ?? 0) * c.Amount
        ));
    }
}
