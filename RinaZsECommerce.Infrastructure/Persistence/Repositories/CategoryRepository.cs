using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category, BaseFilter>, ICategoryRepository
{
  public CategoryRepository(AppDbContext context) : base(context) { }
  protected override IQueryable<Category> ApplyFilter(IQueryable<Category> query, BaseFilter filter)
  {
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      query = query.Where(c => c.Name.Contains(filter.SearchTerm));
      query = query.Where(c => c.Description != null && c.Description.Contains(filter.SearchTerm));
    }

    query = filter.SortBy?.ToLower() switch
    {
      "name" => filter.IsAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };
    return query;
  }
}
