using System;
using RinaZsECommerce.Domain.Common;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class UserFilter : BaseFilter
{
  public string? Email { get; set; }
  public string? Role { get; set; }
  public string? Address { get; set; }
  public DateOnly? DateOfBirth { get; set; }
  public string? PhoneNumber { get; set; }
  public bool? Gender { get; set; }
  public bool? Verified { get; set; }
}
