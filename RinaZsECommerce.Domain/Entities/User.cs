using System;
using System.ComponentModel.DataAnnotations;
using RinaZsECommerce.Domain.Attributes;

namespace RinaZsECommerce.Domain.Entities;

public class User : BaseEntity
{
  public required string Username { get; set; }
  public required string Email { get; set; }
  [NotAuditable] public required string PasswordHash { get; set; }
  public required string Role { get; set; }
  public string? Avatar { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Address { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? PhoneNumber { get; set; }
  public bool? Gender { get; set; }
  public bool Verified { get; set; }

  public ICollection<Review> Reviews { get; set; } = [];
  public ICollection<Order> Orders { get; set; } = [];
  public ICollection<StockLog> StockLogs { get; set; } = [];
  public ICollection<AuditLog> AuditLogs { get; set; } = [];
}
