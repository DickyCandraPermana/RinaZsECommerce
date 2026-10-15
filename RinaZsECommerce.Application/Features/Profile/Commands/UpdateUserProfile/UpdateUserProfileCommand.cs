using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Profile.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(Guid UserId, string? FirstName, string? LastName, string? PhoneNumber) : IRequest<bool>;
