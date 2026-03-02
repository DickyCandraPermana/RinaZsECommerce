using System;
using RinaZsECommerce.Domain.Common;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class CartItemFilter : BaseFilter
{
  public bool? Status { get; set; }
  public Guid? UserId { get; set; }
  public Guid? ProductId { get; set; }
}
