using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class RestockConfiguration : IEntityTypeConfiguration<Restock>
{
  public void Configure(EntityTypeBuilder<Restock> builder)
  {
    builder.HasKey(r => r.Id);

    builder.Property(r => r.ProductId)
      .IsRequired();
    builder.Property(r => r.SupplierId)
      .IsRequired();

    builder.HasQueryFilter(r => r.DeletedAt == null);

    builder.HasOne(r => r.Product)
      .WithMany(p => p.Restocks)
      .HasForeignKey(p => p.ProductId)
      .OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(r => r.Supplier)
      .WithMany(s => s.Restocks)
      .HasForeignKey(p => p.SupplierId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
