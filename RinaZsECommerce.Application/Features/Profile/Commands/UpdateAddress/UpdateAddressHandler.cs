using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Profile.Commands.UpdateAddress;

public class UpdateAddressHandler : IRequestHandler<UpdateAddressCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAddressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null || user.UserProfile == null) return false;

        user.UserProfile.Address = request.Address;
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
