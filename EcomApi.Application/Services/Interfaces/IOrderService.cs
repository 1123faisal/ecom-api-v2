using System;
using EcomApi.Application.DTOs.Order;

namespace EcomApi.Application.Services.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync();
    Task<List<OrderResponseDto>> GetByUserIdAsync(int userId);
    Task<OrderResponseDto?> GetByIdAsync(int id);
    Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto);
    Task<OrderResponseDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto);
}
