using System;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Entities;

public class StockLog : BaseEntity
{

  public required int Quantity { get; set; }
  public required int StockBefore { get; set; }
  public required int StockAfter { get; set; }
  public required Guid UserId { get; set; }
  public required Guid ProductId { get; set; }

  public required StockTransactionType TransactionType { get; set; }

  public Guid? ReferenceId { get; set; }
  public string? Note { get; set; }

  public Product Product { get; set; } = null!;
  public UserProfile User { get; set; } = null!;
}
