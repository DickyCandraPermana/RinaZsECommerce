using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
  public void Configure(EntityTypeBuilder<Voucher> builder)
  {
    builder.HasKey(v => v.Id);

    builder.Property(v => v.Amount)
      .HasColumnType("decimal(18,2)");
    builder.Property(v => v.MinSpend)
      .HasColumnType("decimal(18,2)")
      .HasDefaultValue(0);
    builder.Property(v => v.MaxDiscount)
      .HasColumnType("decimal(18,2)");
    builder.Property(v => v.ValidStart)
      .HasColumnType("date");
    builder.Property(v => v.Expired)
      .HasColumnType("date");

    builder.HasOne(v => v.User)
      .WithMany(u => u.Vouchers)
      .HasForeignKey(v => v.UserId);
  }
}
