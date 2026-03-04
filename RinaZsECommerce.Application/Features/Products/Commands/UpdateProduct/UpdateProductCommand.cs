using MediatR;

namespace RinaZsECommerce.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    Guid UserId,
    string? Name,
    decimal? Price,
    int? Stock,
    Guid? CategoryId,
    List<string>? ImageUrls) : IRequest<Guid>;