using System;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Guid>
{
  private readonly IUnitOfWork _unitOfWork;

  public UpdateProductHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Guid> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
  {
    // Gunakan Include jika kamu butuh update gambar/detail sekalian
    var product = await _unitOfWork.Products.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException(nameof(Product));

    // Update Properti (Bisa pakai AutoMapper agar lebih singkat)
    if (request.Name != null) product.Name = request.Name;
    if (request.Price != null) product.Price = request.Price.Value;
    if (request.CategoryId != null) product.CategoryId = request.CategoryId.Value;

    // Logika Stok & Log
    if (request.Stock != null)
    {
      // Hitung selisih (misal: stok lama 10, request 15, berarti diff = 5)
      int diff = request.Stock.Value - product.Stock;

      // Kita panggil fungsi update stok yang juga mencatat log
      await _unitOfWork.Products.UpdateStockAsync(
          product.Id,
          diff,
          request.UserId,
          StockTransactionType.Adjustment);
    }

    // Tidak perlu panggil .Update() jika entity sudah di-track
    await _unitOfWork.CompleteAsync();

    return product.Id;
  }
}
