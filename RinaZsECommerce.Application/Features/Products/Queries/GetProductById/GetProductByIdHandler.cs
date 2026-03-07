using System;
using AutoMapper;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;

  public GetProductByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
  }

  public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
  {
    // 1. Ambil data dari Repository
    // Pastikan Repository sudah melakukan .Include(p => p.Category) dan .Include(p => p.Images)
    var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

    if (product == null)
      throw new KeyNotFoundException(nameof(Product));

    // 2. Map Entity ke Response DTO menggunakan AutoMapper
    return _mapper.Map<ProductResponse>(product);
  }
}