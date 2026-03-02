using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class SupplierRepository : GenericRepository<Supplier, BaseFilter>, ISupplierRepository
{
  public SupplierRepository(AppDbContext context) : base(context) { }

  protected override IQueryable<Supplier> ApplyFilter(IQueryable<Supplier> query, BaseFilter filter)
  {
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      query = query.Where(s => s.Name.Contains(filter.SearchTerm));
      query = query.Where(s => s.Description != null && s.Description.Contains(filter.SearchTerm));
    }

    query = filter.SortBy?.ToLower() switch
    {
      "name" => filter.IsAscending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
      "description" => filter.IsAscending ? query.OrderBy(s => s.Description) : query.OrderByDescending(s => s.Description),
      _ => filter.IsAscending ? query.OrderBy(s => s.CreatedAt) : query.OrderByDescending(s => s.CreatedAt),
    };
    return query;
  }
}
