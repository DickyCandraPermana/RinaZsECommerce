using System;
using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Profile.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileResponse?>;
