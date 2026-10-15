using System.Collections.Generic;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Entities.Filter;

namespace RinaZsECommerce.Application.Features.Admin.Orders.Queries.GetAllOrders;

public record GetAllOrdersAdminQuery(OrderFilter Filter) : IRequest<IEnumerable<OrderResponse>>;
