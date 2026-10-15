using System;
using System.Collections.Generic;
using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Orders.Queries.GetOrderHistory;

public record GetOrderHistoryQuery(Guid UserId) : IRequest<IEnumerable<OrderResponse>>;
