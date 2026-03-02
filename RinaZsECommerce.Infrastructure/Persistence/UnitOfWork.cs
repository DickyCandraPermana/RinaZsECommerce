using System;
using RinaZsECommerce.Domain.Interfaces;
using RinaZsECommerce.Infrastructure.Persistence.Repositories;

namespace RinaZsECommerce.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
  private readonly AppDbContext _context;

  private IProductRepository? _products;
  private ICartItemRepository? _carts;
  private IOrderRepository? _orders;
  private ICategoryRepository? _categories;
  private IReviewRepository? _reviews;
  private ISupplierRepository? _suppliers;
  private IUserRepository? _users;
  private IVoucherRepository? _vouchers;

  public UnitOfWork(AppDbContext context) => _context = context;

  public IProductRepository Products => _products ??= new ProductRepository(_context);
  public ICartItemRepository Carts => _carts ??= new CartItemRepository(_context);
  public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
  public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
  public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
  public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);
  public IUserRepository Users => _users ??= new UserRepository(_context);
  public IVoucherRepository Vouchers => _vouchers ??= new VoucherRepository(_context);

  public async Task<int> CompleteAsync()
  {
    // Di sini SaveChangesAsync dipanggil sekali untuk semua operasi
    return await _context.SaveChangesAsync();
  }

  public void Dispose() => _context.Dispose();


}
