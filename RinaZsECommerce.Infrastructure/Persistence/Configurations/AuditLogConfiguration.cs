using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
  public void Configure(EntityTypeBuilder<AuditLog> builder)
  {
    builder.HasKey(l => l.Id);

    builder.Property(l => l.Action).IsRequired();
    builder.Property(l => l.EntityName).IsRequired();
    builder.Property(l => l.EntityId).IsRequired();

    builder.HasQueryFilter(p => p.DeletedAt == null);
    
    builder.HasOne(l => l.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(l => l.EntityName);
  }
}
