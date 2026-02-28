using System;
using System.ComponentModel.DataAnnotations;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Entities;

public class Payment : BaseEntity
{
  public required Guid OrderId { get; set; }
  public string? PaymentMethod { get; set; }
  public PaymentStatus Status { get; set; }

  public required Order Order { get; set; }
}
