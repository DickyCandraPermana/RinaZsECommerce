using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Helper;

public class AuditEntry
{
  public AuditEntry(EntityEntry entry) => Entry = entry;
  public EntityEntry Entry { get; }
  public string UserId { get; set; } = "System";
  public string TableName { get; set; } = null!;
  public Dictionary<string, object> OldValues { get; } = new();
  public Dictionary<string, object> NewValues { get; } = new();
  public AuditAction Action { get; set; }
  public List<string> ChangedColumns { get; } = new();

  public AuditLog ToAuditLog() => new AuditLog
  {
    UserId = UserId,
    Action = Action.ToString(),
    EntityName = TableName,
    EntityId = Entry.Properties.First(p => p.Metadata.IsPrimaryKey()).CurrentValue?.ToString() ?? "Unknown",
    CreatedAt = DateTime.UtcNow,
    OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
    NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues)
  };
}

public enum AuditAction { Create, Update, Delete }