using System;
using Microsoft.EntityFrameworkCore;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User, UserFilter>, IUserRepository
{
  public UserRepository(AppDbContext context) : base(context) { }

  public override async Task<User?> GetByIdAsync(Guid id)
  {
    return await _dbSet
        .Include(u => u.UserProfile)
        .FirstOrDefaultAsync(u => u.Id == id);
  }

  protected override IQueryable<User> ApplyFilter(IQueryable<User> query, UserFilter filter)
  {
    // Selalu include Profile untuk semua list user (jika dibutuhkan)
    query = query.Include(u => u.UserProfile);

    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      // Search berdasarkan Username atau Nama Lengkap di Profile
      query = query.Where(u => u.Username.Contains(filter.SearchTerm));
      query = query.Where(u => (u.UserProfile.FirstName ?? "").Contains(filter.SearchTerm));
      query = query.Where(u => (u.UserProfile.LastName ?? "").Contains(filter.SearchTerm));
    }

    if (!string.IsNullOrEmpty(filter.Email))
    {
      query = query.Where(u => u.Email == filter.Email);
    }

    if (!string.IsNullOrEmpty(filter.Role))
    {
      query = query.Where(u => u.Role == filter.Role);
    }

    if (!string.IsNullOrEmpty(filter.Address))
    {
      query = query.Where(u => u.UserProfile.Address == filter.Address);
    }

    if (filter.DateOfBirth != null)
    {
      query = query.Where(u => u.UserProfile.DateOfBirth == filter.DateOfBirth);
    }

    if (!string.IsNullOrEmpty(filter.PhoneNumber))
    {
      query = query.Where(u => u.UserProfile.PhoneNumber == filter.PhoneNumber);
    }

    if (filter.Gender != null)
    {
      query = query.Where(u => u.UserProfile.Gender == filter.Gender);
    }

    if (filter.Verified != null)
    {
      query = query.Where(u => u.UserProfile.Verified == filter.Verified);
    }

    // Contoh Sorting Sederhana
    query = filter.SortBy?.ToLower() switch
    {
      "username" => filter.IsAscending ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username),
      "email" => filter.IsAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
      _ => query.OrderByDescending(u => u.CreatedAt)
    };

    return query;
  }

  public async Task<bool> IsEmailUniqueAsync(string email)
  {
    return !await _dbSet.AnyAsync(u => u.Email == email);
  }
}
