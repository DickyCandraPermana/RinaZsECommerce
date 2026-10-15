using System.Collections.Generic;
using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<IEnumerable<CategoryResponse>>;
