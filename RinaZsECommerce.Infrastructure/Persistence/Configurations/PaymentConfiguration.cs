using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
  public void Configure(EntityTypeBuilder<Payment> builder)
  {
    builder.HasKey(p => p.Id);

    builder.HasQueryFilter(p => p.DeletedAt == null);

    builder.Property(p => p.PaymentMethod)
      .IsRequired();
    builder.Property(p => p.OrderId).IsRequired();

    builder.Property(p => p.Status)
      .HasConversion<string>()
      .HasMaxLength(50)
      .HasDefaultValue(PaymentStatus.Pending);

    builder.HasOne(p => p.Order)
      .WithOne(o => o.Payment)
      .HasForeignKey<Payment>(p => p.OrderId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
