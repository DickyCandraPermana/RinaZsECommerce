using System;
using RinaZsECommerce.Domain.Attributes;

namespace RinaZsECommerce.Domain.Entities;

public class User : BaseEntity
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  [NotAuditable] public required string PasswordHash { get; set; }
  public required string Role { get; set; }
  public Guid ProfileId { get; set; }

  public UserProfile UserProfile { get; set; } = null!;
}
