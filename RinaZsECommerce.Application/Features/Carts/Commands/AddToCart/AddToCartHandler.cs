using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Carts.Commands.AddToCart;

public class AddToCartHandler : IRequestHandler<AddToCartCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // For MVP: Get UserProfile ID since CartItem might be linked to UserProfile. 
        // Let's check Domain/Entities/CartItem.cs relationship to see if it links to ProfileId or UserId.
        // Assuming UserId for now based on typical design, but let's just add it.
        
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new Exception("User not found");

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = user.ProfileId, // Based on UserProfile structure
            ProductId = request.ProductId,
            Amount = request.Quantity,
            Product = null! // Navigation property
        };

        await _unitOfWork.Carts.AddAsync(cartItem);
        await _unitOfWork.CompleteAsync();

        return cartItem.Id;
    }
}
