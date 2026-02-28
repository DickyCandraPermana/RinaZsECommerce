using System;
using RinaZsECommerce.Domain.Common;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class ProductFilter : BaseFilter
{
  public decimal? MaxPrice { get; set; }
  public decimal? MinPrice { get; set; }
  public Guid? CategoryId { get; set; }
  public Guid? SupplierId { get; set; }
}
