using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Profile.Commands.UpdateAddress;

public record UpdateAddressCommand(Guid UserId, string Address) : IRequest<bool>;
