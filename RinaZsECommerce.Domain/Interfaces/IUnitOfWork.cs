using System;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IAuditLogRepository AuditLogs { get; }
  IProductRepository Products { get; }
  ICartItemRepository Carts { get; }
  IOrderRepository Orders { get; }
  ICategoryRepository Categories { get; }
  IPaymentRepository Payments { get; }
  IReviewRepository Reviews { get; }
  ISupplierRepository Suppliers { get; }
  IUserRepository Users { get; }
  IVoucherRepository Vouchers { get; }

  Task<int> CompleteAsync();
}