using System;

namespace RinaZsECommerce.Domain.Common;

public class BaseFilter
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? SearchTerm { get; set; }
  public string? SortBy { get; set; }
  public bool IsAscending { get; set; } = true;
}