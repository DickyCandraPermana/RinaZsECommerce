using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
       public void Configure(EntityTypeBuilder<Order> builder)
       {
              builder.HasKey(o => o.Id);

              builder.HasQueryFilter(p => p.DeletedAt == null);

              builder.Property(o => o.UserId).IsRequired();
              builder.Property(o => o.TotalAmount)
                     .IsRequired()
                     .HasColumnType("decimal(18,2)");
              builder.Property(o => o.Address).IsRequired();
              builder.Property(o => o.Status)
                     .HasConversion<string>()
                     .HasMaxLength(50)
                     .HasDefaultValue(OrderStatus.Pending);
              builder.Property(o => o.UserId).IsRequired();

              builder.HasOne(o => o.User)
                     .WithMany(u => u.Orders)
                     .HasForeignKey(o => o.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
       }
}
