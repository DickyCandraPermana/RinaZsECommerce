using System;
using RinaZsECommerce.Domain.Common;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IGenericRepository<T, TFilter>
  where T : class
  where TFilter : class
{
  Task<T?> GetByIdAsync(Guid id);

  Task<PagedResult<T>> GetPaginatedAsync(TFilter filter);

  Task<IEnumerable<T>> GetAllAsync();
  Task AddAsync(T entity);
  void Update(T entity);
  void Delete(T entity);
}
