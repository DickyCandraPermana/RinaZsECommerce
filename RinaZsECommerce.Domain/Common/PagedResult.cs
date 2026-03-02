using System;

namespace RinaZsECommerce.Domain.Common;

public class PagedResult<T>
{
  public IEnumerable<T> Items { get; set; } = new List<T>();
  public int TotalItems { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

  public static PagedResult<T> Create(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize)
  {
    return new PagedResult<T>
    {
      Items = items,
      TotalItems = totalItems,
      PageNumber = pageNumber,
      PageSize = pageSize
    };
  }
}