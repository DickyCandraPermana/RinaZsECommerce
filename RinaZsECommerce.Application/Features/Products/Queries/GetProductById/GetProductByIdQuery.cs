using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Products.Queries.GetProductById;


// Query adalah record yang berisi parameter pencarian
public record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse>;