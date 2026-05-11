namespace EcomApi.Application.DTOs.Order;

public record CreateOrderItemDto(int ProductId, int Quantity);

public record CreateOrderDto(List<CreateOrderItemDto> Items);

public record OrderItemResponseDto(
    int ProductId,
    string ProductName,
    int Quantity,
    double UnitPrice,
    double Subtotal
);

public record OrderResponseDto(
    int Id,
    int UserId,
    string Username,
    DateTime OrderedAt,
    string Status,
    double TotalAmount,
    List<OrderItemResponseDto> Items
);

public record UpdateOrderStatusDto(string Status);
