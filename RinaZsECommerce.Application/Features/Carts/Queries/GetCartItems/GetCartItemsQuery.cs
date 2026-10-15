using System;
using System.Collections.Generic;
using MediatR;

namespace RinaZsECommerce.Application.Features.Carts.Queries.GetCartItems;

public record GetCartItemsQuery(Guid UserId) : IRequest<IEnumerable<CartItemDto>>;
