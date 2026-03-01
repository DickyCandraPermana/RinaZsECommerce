using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
  public void Configure(EntityTypeBuilder<CartItem> builder)
  {
    builder.HasKey(c => c.Id);

    builder.Property(c => c.Amount).IsRequired();
    builder.Property(c => c.UserId).IsRequired();
    builder.Property(c => c.ProductId).IsRequired();

    builder.HasQueryFilter(p => p.DeletedAt == null);

    builder.HasOne(c => c.User)
            .WithMany(u => u.CartItems)
            .HasForeignKey(c => c.UserId);
    builder.HasOne(c => c.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(c => c.ProductId);
  }
}
