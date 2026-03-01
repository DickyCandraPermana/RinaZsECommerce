using System;

namespace RinaZsECommerce.Domain.Entities;

public class Supplier : BaseEntity
{
  public required string Name { get; set; }
  public required string Description { get; set; }

  public ICollection<Product> Products { get; set; } = new HashSet<Product>();
  public ICollection<Restock> Restocks { get; set; } = new HashSet<Restock>();
}
