using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Carts.Commands.UpdateQuantity;

public class UpdateCartItemQuantityHandler : IRequestHandler<UpdateCartItemQuantityCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCartItemQuantityHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Kuantitas produk harus lebih dari 0.");

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) throw new KeyNotFoundException("User tidak ditemukan.");

        var cartItem = await _unitOfWork.Carts.GetByIdAsync(request.CartItemId);
        if (cartItem == null || cartItem.UserId != user.ProfileId)
            throw new KeyNotFoundException("Item keranjang belanja tidak ditemukan.");

        var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
        if (product == null) throw new KeyNotFoundException("Produk tidak ditemukan.");

        if (product.Stock < request.Quantity)
            throw new InvalidOperationException($"Stok barang tidak mencukupi. Tersisa {product.Stock} unit.");

        cartItem.Amount = request.Quantity;
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
