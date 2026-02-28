using System;

namespace RinaZsECommerce.Domain.Entities;

public class Review : BaseEntity
{
  public Guid UserId { get; set; }
  public Guid ProductId { get; set; }
  public required string Title { get; set; }
  public required int Rating { get; set; }
  public string[] Tags { get; set; } = [];
  public string? Comment { get; set; }

  public User? User { get; set; }
  public Product? Product { get; set; }
}
