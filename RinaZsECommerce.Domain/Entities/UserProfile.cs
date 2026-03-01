using System;
using System.ComponentModel.DataAnnotations;
using RinaZsECommerce.Domain.Attributes;

namespace RinaZsECommerce.Domain.Entities;

public class UserProfile : BaseEntity
{
  public Guid UserId { get; set; }
  public string? Avatar { get; set; }
  public string? FirstName { get; set; }
  public string? LastName { get; set; }
  public string? Address { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? PhoneNumber { get; set; }
  public bool? Gender { get; set; }
  public bool Verified { get; set; }

  public User User { get; set; } = null!;
  public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
  public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
  public ICollection<Voucher> Vouchers { get; set; } = new HashSet<Voucher>();
  public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
  public ICollection<StockLog> StockLogs { get; set; } = new HashSet<StockLog>();
  public ICollection<AuditLog> AuditLogs { get; set; } = new HashSet<AuditLog>();
}
