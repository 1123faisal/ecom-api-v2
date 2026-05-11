using System;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;
using EcomApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return await GetByIdAsync(order.Id) ?? order;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db
            .Orders.Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db
            .Orders.Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _db
            .Orders.Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<Order?> UpdateStatusAsync(int id, string Status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null)
            return null;
        order.Status = Status;
        await _db.SaveChangesAsync();
        return await GetByIdAsync(id) ?? order;
    }
}
