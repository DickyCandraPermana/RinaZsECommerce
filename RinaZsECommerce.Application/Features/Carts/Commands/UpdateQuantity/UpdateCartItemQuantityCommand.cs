using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Carts.Commands.UpdateQuantity;

public record UpdateCartItemQuantityCommand(Guid UserId, Guid CartItemId, int Quantity) : IRequest<bool>;
