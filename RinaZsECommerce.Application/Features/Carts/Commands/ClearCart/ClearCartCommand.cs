using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Carts.Commands.ClearCart;

public record ClearCartCommand(Guid UserId) : IRequest<bool>;
