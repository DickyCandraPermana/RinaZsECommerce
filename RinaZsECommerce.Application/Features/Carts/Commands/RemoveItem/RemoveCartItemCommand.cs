using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Carts.Commands.RemoveItem;

public record RemoveCartItemCommand(Guid UserId, Guid CartItemId) : IRequest<bool>;
