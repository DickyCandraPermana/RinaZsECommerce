using System;

namespace RinaZsECommerce.Domain.Entities;

public class Supplier : BaseEntity
{
  public required string Name { get; set; }
  public required string Description { get; set; }
}
