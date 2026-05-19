using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;
using EcomApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User> AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> ExistsAsync(string username, string email, CancellationToken ct = default)
    {
        return await _db.Users.AnyAsync(
            u => u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == email.ToLower(),
            ct
        );
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), ct);
    }
}

