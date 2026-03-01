using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RinaZsECommerce.Domain.Entities;

public class OrderDetail : BaseEntity
{
  public required Guid OrderId { get; set; }
  public required Guid ProductId { get; set; }
  public required int Amount { get; set; }
  public required decimal PriceAtPurchase { get; set; }

  public Order Order { get; set; } = null!;
  public required Product Product { get; set; }
}
