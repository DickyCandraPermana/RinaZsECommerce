using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Admin.Products.Commands.Restock;

public record RestockProductCommand(Guid AdminUserId, Guid ProductId, int Amount, Guid? SupplierId) : IRequest<bool>;
