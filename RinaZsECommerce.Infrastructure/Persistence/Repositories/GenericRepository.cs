using System;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public abstract class GenericRepository<T, TFilter> : IGenericRepository<T, TFilter>
    where T : class
    where TFilter : BaseFilter
{
  protected readonly AppDbContext _context;
  protected readonly DbSet<T> _dbSet;

  protected GenericRepository(AppDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

  public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

  public virtual async Task<PagedResult<T>> GetPaginatedAsync(TFilter filter)
  {
    IQueryable<T> query = _dbSet;

    // Apply custom filters (logic ada di child class)
    query = ApplyFilter(query, filter);

    // Hitung total sebelum di-skip/take
    var totalItems = await query.CountAsync();

    // Apply Pagination
    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();

    return PagedResult<T>.Create(
        items,
        totalItems,
        filter.PageNumber,
        filter.PageSize);
  }

  protected abstract IQueryable<T> ApplyFilter(IQueryable<T> query, TFilter filter);

  public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

  public void Update(T entity) => _dbSet.Update(entity);

  public void Delete(T entity) => _dbSet.Remove(entity);
}