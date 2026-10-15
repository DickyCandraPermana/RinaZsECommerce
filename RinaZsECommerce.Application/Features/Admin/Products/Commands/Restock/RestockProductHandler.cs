using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Admin.Products.Commands.Restock;

public class RestockProductHandler : IRequestHandler<RestockProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RestockProductHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RestockProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Jumlah penambahan stok harus lebih dari 0.");

        var admin = await _unitOfWork.Users.GetByIdAsync(request.AdminUserId);
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
        if (product == null) throw new KeyNotFoundException("Produk tidak ditemukan.");

        var supplierId = request.SupplierId ?? product.SupplierId;

        // Catat Restock
        await _unitOfWork.Products.UpdateStockAsync(
            product.Id,
            request.Amount,
            admin?.ProfileId ?? Guid.Empty,
            StockTransactionType.Restock);

        await _unitOfWork.CompleteAsync();
        return true;
    }
}
