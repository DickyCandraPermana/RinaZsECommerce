using System;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class VoucherFilter
{
  public decimal? MinAmount { get; set; }
  public decimal? MaxAmount { get; set; }
  public decimal? MinSpend { get; set; }
  public decimal? MaxDiscount { get; set; }
  public Guid? UserId { get; set; }
  public DateOnly? ValidStart { set; get; }
  public DateOnly? Expired { get; set; }
}
