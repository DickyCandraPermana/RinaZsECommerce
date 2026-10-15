using System;
using System.Collections.Generic;

namespace RinaZsECommerce.Application.DTOs;

public record OrderDetailResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Amount,
    decimal PriceAtPurchase,
    decimal Subtotal
);

public record OrderResponse(
    Guid Id,
    Guid UserId,
    string CustomerName,
    string CustomerEmail,
    string Address,
    string? Description,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderDetailResponse> Items
);
