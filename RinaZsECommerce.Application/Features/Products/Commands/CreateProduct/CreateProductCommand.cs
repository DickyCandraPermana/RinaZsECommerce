using MediatR;

namespace RinaZsECommerce.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    decimal Price,
    int Stock,
    Guid CategoryId,
    List<string> ImageUrls) : IRequest<Guid>;