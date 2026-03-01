using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Username)
      .HasMaxLength(20)
      .IsRequired();
    builder.Property(u => u.Email)
      .HasColumnType("varchar(256)")
      .IsRequired()
      .HasMaxLength(256);
    builder.Property(u => u.PasswordHash)
      .IsRequired();
    builder.Property(u => u.Role)
      .HasDefaultValue("User");

    builder.HasQueryFilter(u => u.DeletedAt == null);

    builder.HasIndex(u => u.Email)
      .IsUnique();
    builder.HasIndex(u => u.Username)
      .IsUnique();

    builder.Property(u => u.Email)
      .HasConversion(
          v => v.ToLower(),
          v => v
      );
    builder.Property(u => u.Username)
      .HasConversion(
          v => v.ToLower(),
          v => v
      );
  }
}
