using System;

namespace RinaZsECommerce.Domain.Entities;

public class Restock : BaseEntity
{
  public required Guid ProductId { get; set; }
  public required Guid SupplierId { get; set; }
  public required int Amount { get; set; }
  
}
