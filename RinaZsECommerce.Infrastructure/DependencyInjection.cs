using System;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Interfaces;
using RinaZsECommerce.Infrastructure.Persistence;

namespace RinaZsECommerce.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<AppDbContext>(options =>
      options.UseNpgsql(connectionString));

    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}
