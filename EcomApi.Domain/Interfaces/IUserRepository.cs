using System;
using EcomApi.Domain.Entities;

namespace EcomApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string username, string email);
    Task<User> AddAsync(User user);
}
