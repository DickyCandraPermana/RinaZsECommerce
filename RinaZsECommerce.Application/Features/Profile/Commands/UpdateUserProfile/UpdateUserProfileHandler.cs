using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Profile.Commands.UpdateUserProfile;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null || user.UserProfile == null) return false;

        if (request.FirstName != null) user.UserProfile.FirstName = request.FirstName;
        if (request.LastName != null) user.UserProfile.LastName = request.LastName;
        if (request.PhoneNumber != null) user.UserProfile.PhoneNumber = request.PhoneNumber;

        await _unitOfWork.CompleteAsync();
        return true;
    }
}
