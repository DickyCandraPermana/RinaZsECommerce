using System;

namespace RinaZsECommerce.Domain.Entities;

public class AuditLog : BaseEntity
{
  public string? UserId { get; set; }
  public required string Action { get; set; }
  public required string EntityName { get; set; }
  public required string EntityId { get; set; }

  public string? OldValues { get; set; } // JSON string
  public string? NewValues { get; set; } // JSON string

  public string? IpAddress { get; set; }

  public UserProfile? User { get; set; }
}
