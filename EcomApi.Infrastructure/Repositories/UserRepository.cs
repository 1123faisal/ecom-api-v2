using System;
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

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(user.Id) ?? user;
    }

    public async Task<bool> ExistsAsync(string username, string email)
    {
        return await _db.Users.AnyAsync(u =>
            u.Username.ToLower() == username.ToLower() || u.Email.ToLower() == email.ToLower()
        );
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(p => p.Username.ToLower() == username.ToLower());
    }
}
