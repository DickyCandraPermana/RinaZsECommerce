using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RinaZsECommerce.Domain.Entities;

public class Product : BaseEntity
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public required int Stock { get; set; }
  public required decimal Price { get; set; }
  public required bool Status { get; set; }
  public required Guid CategoryId { get; set; }

  public Category Category { get; set; } = null!;
  public ICollection<ProductImage> Images { get; set; } = [];
  public ICollection<Order> Orders { get; set; } = [];
  public ICollection<StockLog> StockLogs { get; set; } = [];
}
