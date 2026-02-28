using System;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class UserFilter
{
  public string? Role { get; set; }
  public string? Address { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? PhoneNumber { get; set; }
  public bool? Gender { get; set; }
  public bool? Verified { get; set; }
}
