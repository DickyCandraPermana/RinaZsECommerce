using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid UserId, Guid OrderId) : IRequest<bool>;
