using System;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Infrastructure.Helper;

namespace RinaZsECommerce.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
  public DbSet<CartItem> CartItems => Set<CartItem>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Order> Orders => Set<Order>();
  public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
  public DbSet<Payment> Payments => Set<Payment>();
  public DbSet<Product> Products => Set<Product>();
  public DbSet<ProductImage> ProductImages => Set<ProductImage>();
  public DbSet<Restock> Restocks => Set<Restock>();
  public DbSet<Review> Reviews => Set<Review>();
  public DbSet<StockLog> StockLogs => Set<StockLog>();
  public DbSet<Supplier> Suppliers => Set<Supplier>();
  public DbSet<UserProfile> Users => Set<UserProfile>();
  public DbSet<Voucher> Vouchers => Set<Voucher>();

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    foreach (var entry in ChangeTracker.Entries<BaseEntity>())
    {
      switch (entry.State)
      {
        case EntityState.Added:
          entry.Entity.CreatedAt = DateTime.UtcNow;
          break;
        case EntityState.Modified:
          entry.Entity.UpdatedAt = DateTime.UtcNow;
          break;
        case EntityState.Deleted:
          entry.State = EntityState.Modified;
          entry.Entity.DeletedAt = DateTime.UtcNow;
          break;
      }
    }

    var auditEntries = OnBeforeSaveChanges();

    var result = await base.SaveChangesAsync(cancellationToken);

    await OnAfterSaveChanges(auditEntries);
    return result;
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    // Otomatis load semua IEntityTypeConfiguration di assembly ini
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }

  private List<AuditEntry> OnBeforeSaveChanges()
  {
    ChangeTracker.DetectChanges();
    var auditEntries = new List<AuditEntry>();

    foreach (var entry in ChangeTracker.Entries())
    {
      if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        continue;

      var auditEntry = new AuditEntry(entry)
      {
        TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name
      };
      auditEntries.Add(auditEntry);

      foreach (var property in entry.Properties)
      {
        string propertyName = property.Metadata.Name;
        if (property.Metadata.IsPrimaryKey()) continue;

        switch (entry.State)
        {
          case EntityState.Added:
            auditEntry.Action = AuditAction.Create;
            auditEntry.NewValues[propertyName] = property.CurrentValue!;
            break;
          case EntityState.Deleted:
            auditEntry.Action = AuditAction.Delete;
            auditEntry.OldValues[propertyName] = property.OriginalValue!;
            break;
          case EntityState.Modified:
            if (property.IsModified)
            {
              auditEntry.Action = AuditAction.Update;
              auditEntry.OldValues[propertyName] = property.OriginalValue!;
              auditEntry.NewValues[propertyName] = property.CurrentValue!;
            }
            break;
        }
      }
    }
    return auditEntries;
  }

  private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
  {
    if (auditEntries == null || auditEntries.Count == 0) return Task.CompletedTask;

    foreach (var auditEntry in auditEntries)
    {
      AuditLogs.Add(auditEntry.ToAuditLog());
    }
    return base.SaveChangesAsync();
  }

}
