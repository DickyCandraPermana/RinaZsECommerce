using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Carts.Commands.RemoveItem;

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCartItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) throw new KeyNotFoundException("User tidak ditemukan.");

        var cartItem = await _unitOfWork.Carts.GetByIdAsync(request.CartItemId);
        if (cartItem == null || cartItem.UserId != user.ProfileId)
            return false;

        _unitOfWork.Carts.Delete(cartItem);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
