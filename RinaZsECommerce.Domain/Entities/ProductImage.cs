using System;

namespace RinaZsECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
  public required Guid ProductId { get; set; }
  public required string ImageUrl { get; set; }
  public string ImageAltDesc { get; set; } = string.Empty;

  public Product? Product { get; set; }
}
