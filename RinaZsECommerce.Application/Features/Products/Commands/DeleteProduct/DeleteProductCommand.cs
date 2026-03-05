using MediatR;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Application.Features.Products.Commands.DeleteProduct;

public record class DeleteProductCommand(Guid ProductId) : IRequest<bool>;
