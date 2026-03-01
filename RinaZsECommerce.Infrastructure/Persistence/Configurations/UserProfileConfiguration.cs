using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
  public void Configure(EntityTypeBuilder<UserProfile> builder)
  {
    builder.HasKey(u => u.Id);

    builder.Property(u => u.Verified)
      .HasDefaultValue(false);
    builder.Property(u => u.DateOfBirth)
      .HasColumnType("date");

    builder.HasOne(u => u.User)
      .WithOne(p => p.UserProfile)
      .HasForeignKey<UserProfile>(u => u.UserId)
      .OnDelete(DeleteBehavior.Restrict);


  }

}
