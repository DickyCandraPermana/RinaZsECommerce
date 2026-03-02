using System;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class CartItemRepository : GenericRepository<CartItem, CartItemFilter>, ICartItemRepository
{
  public CartItemRepository(AppDbContext context) : base(context) { }

  protected override IQueryable<CartItem> ApplyFilter(IQueryable<CartItem> query, CartItemFilter filter)
  {
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      query = query.Where(c => c.Product.Name.Contains(filter.SearchTerm));
    }
    if (filter.UserId != null)
    {
      query = query.Where(c => c.UserId == filter.UserId);
    }

    if (filter.ProductId != null)
    {
      query = query.Where(c => c.ProductId == filter.ProductId);
    }

    if (filter.Status != null)
    {
      query = query.Where(c => c.Product.Status == filter.Status);
    }

    query = filter.SortBy?.ToLower() switch
    {
      "product" => filter.IsAscending ? query.OrderBy(c => c.Product.Name) : query.OrderByDescending(c => c.Product.Name),
      "price" => filter.IsAscending ? query.OrderBy(c => c.Product.Price) : query.OrderByDescending(c => c.Product.Price),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }

  public async Task ClearCartAsync(Guid userId)
  {
    await _dbSet
        .Where(c => c.UserId == userId)
        .ExecuteDeleteAsync();
  }

}
