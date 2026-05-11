using System;
using EcomApi.Application.DTOs.Order;
using EcomApi.Application.Services.Interfaces;
using EcomApi.Domain.Entities;
using EcomApi.Domain.Interfaces;

namespace EcomApi.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> CreateAsync(int userId, CreateOrderDto dto)
    {
        var orderItems = new List<OrderItem>();
        double totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new Exception($"Product {item.ProductId} not found.");

            if (product.Stock < item.Quantity)
                throw new Exception(
                    $"Insufficient stock for {product.Name}. Available: {product.Stock}, Requested:{item.Quantity}"
                );

            orderItems.Add(
                new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                }
            );

            totalAmount += product.Price * item.Quantity;

            await _productRepository.UpdateStockAsync(
                item.ProductId,
                product.Stock - item.Quantity
            );
        }

        var order = new Order
        {
            UserId = userId,
            TotalAmount = totalAmount,
            Status = "Pending",
            OrderItems = orderItems,
        };

        var created = await _orderRepository.CreateAsync(order);

        return MapToDto(created);
    }

    public async Task<List<OrderResponseDto>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<List<OrderResponseDto>> GetByUserIdAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto?> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
    {
        var validStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        if (!validStatuses.Contains(dto.Status))
            throw new Exception(
                $"Invalid status. valid values:.{string.Join(", ", validStatuses)}"
            );

        var order = await _orderRepository.UpdateStatusAsync(id, dto.Status);
        return order == null ? null : MapToDto(order);
    }

    private OrderResponseDto MapToDto(Order order) =>
        new(
            order.Id,
            order.UserId,
            order.User?.Username ?? "Unknown",
            order.OrderedAt,
            order.Status,
            order.TotalAmount,
            order.OrderItems.Select(MapToOrderItemDto).ToList()
        );

    private OrderItemResponseDto MapToOrderItemDto(OrderItem orderItem) =>
        new(
            orderItem.ProductId,
            orderItem.Product?.Name ?? "Unknown",
            orderItem.Quantity,
            orderItem.UnitPrice,
            orderItem.Quantity * orderItem.UnitPrice
        );
}
