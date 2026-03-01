using System;
using System.ComponentModel.DataAnnotations;

namespace RinaZsECommerce.Domain.Entities;

public class CartItem : BaseEntity
{
  public required int Amount { get; set; }
  public required Guid UserId { get; set; }
  public required Guid ProductId { get; set; }

  public UserProfile User { get; set; } = null!;
  public required Product Product { get; set; }
}
