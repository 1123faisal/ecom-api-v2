using EcomApi.Domain.Entities;
using EcomApi.Domain.Enums;
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

    public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(order.Id, ct) ?? order;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _db.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<Order?> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct = default)
    {
        var order = await _db.Orders.FindAsync([id], ct);
        if (order == null)
            return null;

        order.Status = status;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct) ?? order;
    }
}

