using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Entities;

public class Order : BaseEntity
{
  public required Guid UserId { get; set; }
  public string? Description { get; set; }
  public required decimal TotalAmount { get; set; }
  public required string Address { get; set; }
  public OrderStatus Status { get; set; }

  public required UserProfile User { get; set; }
  public Payment? Payment { get; set; }
  public required ICollection<OrderDetail> Details { get; set; } = new HashSet<OrderDetail>();
}
