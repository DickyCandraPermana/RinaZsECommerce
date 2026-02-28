using System;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class AuditLogFilter
{
  public Guid? UserId { get; set; }
  public string? Action { get; set; }
  public string? Entity { get; set; }
  public string? EntityId { get; set; }
  public string? IpAddress { get; set; }
}
