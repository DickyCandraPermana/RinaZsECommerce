using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Profile.Queries.GetUserProfile;

public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserProfileHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileResponse?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return null;

        return new UserProfileResponse(
            user.Id,
            user.ProfileId,
            user.Username,
            user.Email,
            user.Role,
            user.UserProfile?.FirstName,
            user.UserProfile?.LastName,
            user.UserProfile?.Address,
            user.UserProfile?.PhoneNumber,
            user.UserProfile?.Verified ?? false
        );
    }
}
