using System;
using MediatR;

namespace RinaZsECommerce.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<Guid>;
