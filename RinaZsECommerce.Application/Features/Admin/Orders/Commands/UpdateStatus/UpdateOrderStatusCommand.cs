using System;
using MediatR;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Application.Features.Admin.Orders.Commands.UpdateStatus;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<bool>;
