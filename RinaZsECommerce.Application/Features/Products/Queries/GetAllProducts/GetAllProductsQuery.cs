using MediatR;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(ProductFilter Filter) : IRequest<IEnumerable<ProductResponse>>;
