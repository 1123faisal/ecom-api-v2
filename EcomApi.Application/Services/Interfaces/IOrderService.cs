using EcomApi.Application.DTOs.Order;

namespace EcomApi.Application.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<List<OrderResponseDto>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto, CancellationToken ct = default);
    Task<OrderResponseDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken ct = default);
}
