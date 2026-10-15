using System;

namespace RinaZsECommerce.Application.Features.Carts.Queries.GetCartItems;

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    decimal Price,
    int Stock,
    int Quantity,
    decimal Subtotal
);
