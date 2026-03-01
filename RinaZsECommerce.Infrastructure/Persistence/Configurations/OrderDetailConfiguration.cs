using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
       public void Configure(EntityTypeBuilder<OrderDetail> builder)
       {
              builder.HasKey(d => d.Id);

              builder.Property(d => d.Amount)
                     .IsRequired();
              builder.Property(d => d.PriceAtPurchase)
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();
              builder.Property(d => d.OrderId)
                     .IsRequired();

              builder.HasQueryFilter(p => p.DeletedAt == null);


              builder.HasOne(d => d.Order)
           .WithMany(o => o.Details)
           .HasForeignKey(d => d.OrderId);
              builder.HasOne(d => d.Product)
                     .WithMany(p => p.Orders)
                     .HasForeignKey(d => d.ProductId);

       }
}
