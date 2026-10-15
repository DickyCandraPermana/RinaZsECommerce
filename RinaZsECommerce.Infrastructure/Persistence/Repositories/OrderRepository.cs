using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order, OrderFilter>, IOrderRepository
{
  public OrderRepository(AppDbContext context) : base(context) { }

  public override async Task<Order?> GetByIdAsync(Guid id)
  {
    return await _dbSet
        .Include(o => o.Details)
        .ThenInclude(d => d.Product)
        .ThenInclude(p => p.Images)
        .Include(o => o.User)
        .ThenInclude(u => u.User)
        .FirstOrDefaultAsync(o => o.Id == id);
  }

  public async Task<IEnumerable<Order>> GetOrdersWithDetailsAsync()
  {
    return await _dbSet
        .Include(o => o.Details)
        .ThenInclude(d => d.Product)
        .ThenInclude(p => p.Images)
        .Include(o => o.User)
        .ThenInclude(u => u.User)
        .ToListAsync();
  }

  public async Task<OrderDetail?> GetDetailByIdAsync(Guid orderDetailId)
  {
    return await _dbSet
        .SelectMany(o => o.Details)
        .FirstOrDefaultAsync(d => d.Id == orderDetailId);
  }

  public async Task<OrderDetail?> GetDetailWithOrderAsync(Guid orderDetailId)
  {
    return await _context.Set<OrderDetail>()
        .Include(d => d.Order)
        .Include(d => d.Product)
        .FirstOrDefaultAsync(d => d.Id == orderDetailId);
  }

  protected override IQueryable<Order> ApplyFilter(IQueryable<Order> query, OrderFilter filter)
  {
    query = query
        .Include(o => o.Details)
        .ThenInclude(d => d.Product)
        .ThenInclude(p => p.Images)
        .Include(o => o.User)
        .ThenInclude(u => u.User);

    if (filter.UserId != null)
    {
      query = query.Where(o => o.UserId == filter.UserId);
    }
    if (filter.ProductId != null)
    {
      query = query.Where(o => o.Details.Any(d => d.ProductId == filter.ProductId));
    }
    if (filter.Status != null)
    {
      query = query.Where(o => o.Status == filter.Status);
    }

    query = filter.SortBy?.ToLower() switch
    {
      "user" => filter.IsAscending ? query.OrderBy(o => o.User.User.Username) : query.OrderByDescending(o => o.User.User.Username),
      "email" => filter.IsAscending ? query.OrderBy(o => o.User.User.Email) : query.OrderByDescending(o => o.User.User.Email),
      "address" => filter.IsAscending ? query.OrderBy(o => o.Address) : query.OrderByDescending(o => o.Address),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }
}
