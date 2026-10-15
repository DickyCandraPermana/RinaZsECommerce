using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product, ProductFilter>, IProductRepository
{
  public ProductRepository(AppDbContext context) : base(context) { }

  public override async Task<Product?> GetByIdAsync(Guid id)
  {
    return await _dbSet
        .Include(p => p.Category)
        .Include(p => p.Images)
        .Include(p => p.Supplier)
        .FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task<IEnumerable<Product>> GetActiveProductsAsync()
  {
    return await _dbSet
        .Include(p => p.Category)
        .Include(p => p.Images)
        .Where(p => p.Stock > 0 && p.Status)
        .ToListAsync();
  }

  public async Task<bool> UpdateStockAsync(Guid productId, int diff, Guid userId, StockTransactionType transactionType)
  {
    var product = await _dbSet.FindAsync(productId);
    if (product == null) return false;

    var stockBefore = product.Stock;
    var stockAfter = stockBefore + diff;

    if (stockAfter < 0) return false;

    await _context.StockLogs.AddAsync(new StockLog
    {
      ProductId = productId,
      TransactionType = transactionType,
      Quantity = diff,
      StockBefore = stockBefore,
      StockAfter = stockAfter,
      UserId = userId,
      CreatedAt = DateTime.UtcNow
    });

    product.Stock = stockAfter;
    return true;
  }

  protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductFilter filter)
  {
    query = query.Include(p => p.Category).Include(p => p.Images);

    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      query = query.Where(p => p.Name.ToLower().Contains(filter.SearchTerm.ToLower()) ||
                               (p.Description != null && p.Description.ToLower().Contains(filter.SearchTerm.ToLower())));
    }

    if (filter.MaxPrice >= 0)
    {
      query = query.Where(p => p.Price <= filter.MaxPrice);
    }

    if (filter.MinPrice >= 0)
    {
      query = query.Where(p => p.Price >= filter.MinPrice);
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
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }
}
