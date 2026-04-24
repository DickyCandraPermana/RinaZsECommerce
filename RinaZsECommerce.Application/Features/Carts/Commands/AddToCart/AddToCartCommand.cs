using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Carts.Commands.AddToCart;

public record AddToCartCommand(
    Guid UserId,
    Guid ProductId,
    int Quantity
) : IRequest<Guid>;
