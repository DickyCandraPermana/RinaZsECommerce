using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
  public void Configure(EntityTypeBuilder<Product> builder)
  {
    builder.HasKey(p => p.Id);

    builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
    builder.Property(p => p.Stock).HasDefaultValue(0);
    builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
    builder.Property(p => p.Rating).HasColumnType("decimal(18,2)");
    builder.Property(p => p.Status).HasDefaultValue(true);
    builder.Property(p => p.CategoryId).IsRequired();
    builder.Property(p => p.SupplierId).IsRequired();

    builder.HasQueryFilter(p => p.DeletedAt == null);

    builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

    builder.HasIndex(p => p.Name);
  }
}