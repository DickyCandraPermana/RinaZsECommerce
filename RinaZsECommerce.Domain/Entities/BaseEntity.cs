using System;
using System.ComponentModel.DataAnnotations;

namespace RinaZsECommerce.Domain.Entities;

public abstract class BaseEntity
{
  [Key]
  public Guid Id { get; set; } = Guid.NewGuid();
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; }
  public DateTime? DeletedAt { get; set; }
}
