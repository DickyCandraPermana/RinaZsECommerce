using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
  private readonly IUnitOfWork _unitOfWork;

  public GetProductByIdHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
  {
    var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

    if (product == null)
      throw new KeyNotFoundException($"Product with ID '{request.Id}' was not found.");

    return new ProductResponse(
      product.Id,
      product.Name,
      product.Description,
      product.Price,
      product.Stock,
      product.Rating,
      product.CategoryId,
      product.Category?.Name ?? "Unknown",
      product.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
    );
  }
}
