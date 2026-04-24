using System;
using System.Collections.Generic;
using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Carts.Queries.GetCartItems;

public record GetCartItemsQuery(Guid UserId) : IRequest<IEnumerable<CartItemDto>>;

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Amount,
    decimal Subtotal
);
