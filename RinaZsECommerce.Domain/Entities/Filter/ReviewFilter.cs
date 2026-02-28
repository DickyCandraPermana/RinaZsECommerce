using System;
using RinaZsECommerce.Domain.Common;

namespace RinaZsECommerce.Domain.Entities.Filter;

public class ReviewFilter : BaseFilter
{
  public int? Rating { get; set; }
}
