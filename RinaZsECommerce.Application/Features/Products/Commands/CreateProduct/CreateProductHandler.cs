using System;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
  private readonly IUnitOfWork _unitOfWork;

  public CreateProductHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
  {
    // 1. Map DTO ke Entity (Bisa pakai AutoMapper)
    var product = new Product
    {
      Name = request.Name,
      Price = request.Price,
      Stock = request.Stock,
      CategoryId = request.CategoryId,
      // Ingat fitur "Otomatis" EF Core yang kita bahas? Tinggal masukkan ke List.
      Images = request.ImageUrls.Select(url => new ProductImage { ImageUrl = url, ProductId = default! }).ToList()
    };

    // 2. Tambahkan lewat Repository di dalam UnitOfWork
    await _unitOfWork.Products.AddAsync(product);

    // 3. Eksekusi Transaksi (Supercar Power!)
    // Semua proses (Insert Product & Insert Images) terjadi di sini
    await _unitOfWork.CompleteAsync();

    return product.Id;
  }
}