using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class StockLogConfiguration : IEntityTypeConfiguration<StockLog>
{
  public void Configure(EntityTypeBuilder<StockLog> builder)
  {
    builder.HasKey(l => l.Id);

    builder.Property(l => l.Quantity)
      .IsRequired();
    builder.Property(l => l.StockBefore)
      .IsRequired();
    builder.Property(l => l.StockAfter)
      .IsRequired();
    builder.Property(l => l.ProductId)
      .IsRequired();
    builder.Property(l => l.TransactionType)
      .HasConversion<string>()
      .HasMaxLength(50)
      .IsRequired();

    builder.HasQueryFilter(l => l.DeletedAt == null);

    builder.HasOne(l => l.User)
      .WithMany(u => u.StockLogs)
      .HasForeignKey(l => l.UserId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(l => l.Product)
      .WithMany(p => p.StockLogs)
      .HasForeignKey(l => l.ProductId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
