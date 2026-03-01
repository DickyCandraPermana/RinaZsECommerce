using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
  public void Configure(EntityTypeBuilder<ProductImage> builder)
  {
    builder.HasKey(i => i.Id);

    builder.Property(i => i.ImageUrl)
      .IsRequired();
    builder.Property(i => i.ProductId)
      .IsRequired();

    builder.HasQueryFilter(r => r.DeletedAt == null);

    builder.HasOne(i => i.Product)
      .WithMany(p => p.Images)
      .HasForeignKey(i => i.ProductId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
