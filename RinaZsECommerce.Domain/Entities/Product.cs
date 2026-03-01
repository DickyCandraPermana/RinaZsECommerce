using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RinaZsECommerce.Domain.Entities;

public class Product : BaseEntity
{
  public required string Name { get; set; }
  public string? Description { get; set; }
  public int Stock { get; set; }
  public decimal Price { get; set; }
  public bool Status { get; set; }

  public Guid CategoryId { get; set; }
  public Guid SupplierId { get; set; }

  public Category Category { get; set; } = null!;
  public Supplier Supplier { get; set; } = null!;

  public ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
  public ICollection<OrderDetail> Orders { get; set; } = new HashSet<OrderDetail>();
  public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
  public ICollection<Restock> Restocks { get; set; } = new HashSet<Restock>();
  public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
  public ICollection<StockLog> StockLogs { get; set; } = new HashSet<StockLog>();
}
