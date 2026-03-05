using System;
using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
  private readonly IUnitOfWork _unitOfWork;

  public DeleteProductHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
  {
    var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId) ?? throw new KeyNotFoundException(nameof(Product));

    _unitOfWork.Products.Delete(product);

    await _unitOfWork.CompleteAsync();

    return true;
  }
}
