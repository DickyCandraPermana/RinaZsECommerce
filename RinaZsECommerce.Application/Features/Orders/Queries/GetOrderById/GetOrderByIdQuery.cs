using System;
using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid UserId, Guid OrderId) : IRequest<OrderResponse?>;
