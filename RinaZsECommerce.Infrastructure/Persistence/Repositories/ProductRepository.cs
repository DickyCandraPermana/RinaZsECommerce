using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product, ProductFilter>, IProductRepository
{
  public ProductRepository(AppDbContext context) : base(context) { }

  public async Task<IEnumerable<Product>> GetActiveProductsAsync()
  {
    return await _dbSet.Where(p => p.Stock > 0).ToListAsync();
  }

  public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
  {
    var product = await _dbSet.FindAsync(productId);

    if (product == null || product.Stock < quantity)
    {
      return false;
    }

    product.Stock -= quantity;

    return true;
  }

  protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilter filter)
  {
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      // Search berdasarkan Username atau Nama Lengkap di Profile
      query = query.Where(p => p.Name.Contains(filter.SearchTerm));
    }

    if (filter.MaxPrice >= 0)
    {
      query = query.Where(p => p.Price <= filter.MaxPrice);
    }

    if (filter.MinPrice >= 0)
    {
      query = query.Where(p => p.Price > filter.MinPrice);
    }

    if (filter.Status != null)
    {
      query = query.Where(p => p.Status == filter.Status);
    }

    if (filter.CategoryId != null)
    {
      query = query.Where(p => p.CategoryId == filter.CategoryId);
    }

    if (filter.SupplierId != null)
    {
      query = query.Where(p => p.SupplierId == filter.SupplierId);
    }

    if (filter.Rating > 0)
    {
      query = query.Where(p => p.Rating >= filter.Rating);
    }

    query = filter.SortBy?.ToLower() switch
    {
      "name" => filter.IsAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
      "price" => filter.IsAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
      "rating" => filter.IsAscending ? query.OrderBy(p => p.Rating) : query.OrderByDescending(p => p.Rating),
      "stock" => filter.IsAscending ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock),
      "supplier" => filter.IsAscending ? query.OrderBy(p => p.Supplier.Name) : query.OrderByDescending(p => p.Supplier.Name),
      "category" => filter.IsAscending ? query.OrderBy(p => p.Supplier.Name) : query.OrderByDescending(p => p.Supplier.Name),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }
}
