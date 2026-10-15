using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Carts.Commands.ClearCart;

public class ClearCartHandler : IRequestHandler<ClearCartCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClearCartHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) return false;

        await _unitOfWork.Carts.ClearCartAsync(user.ProfileId);
        return true;
    }
}
