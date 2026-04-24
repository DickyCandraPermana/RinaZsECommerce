using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Orders.Commands.Checkout;

public record CheckoutCommand(
    Guid UserId,
    string Address,
    string? Description
) : IRequest<Guid>;
