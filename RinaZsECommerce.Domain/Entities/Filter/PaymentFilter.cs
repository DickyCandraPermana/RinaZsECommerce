using System;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class PaymentFilter
{
  public Guid? OrderId { get; set; }
  public string? PaymentMethod { get; set; }
  public Guid? UserId { get; set; }
  public Guid? ProductId { get; set; }
}
