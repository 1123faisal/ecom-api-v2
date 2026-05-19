using EcomApi.Domain.Entities;
using EcomApi.Domain.Enums;

namespace EcomApi.Domain.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken ct = default);
    Task<List<Order>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Order> CreateAsync(Order order, CancellationToken ct = default);
    Task<Order?> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct = default);
}
