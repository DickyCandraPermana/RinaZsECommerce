using System;

namespace RinaZsECommerce.Domain.Entities;

public class Voucher : BaseEntity
{
  public string Name { get; set; } = null!;
  public decimal Amount { get; set; }
  public decimal MinSpend { get; set; }
  public decimal? MaxDiscount { get; set; }
  public Guid? UserId { get; set; }
  public DateOnly? ValidStart { set; get; }
  public DateOnly? Expired { get; set; }

  public UserProfile? User { get; set; }
}
