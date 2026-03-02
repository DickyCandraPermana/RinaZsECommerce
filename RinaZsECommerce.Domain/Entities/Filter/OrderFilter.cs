using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class OrderFilter : BaseFilter
{
  public Guid? UserId { get; set; }
  public Guid? ProductId { get; set; }
  public OrderStatus? Status { get; set; }

}
