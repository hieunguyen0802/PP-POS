namespace POS.Application.DTOs.Order;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record CreateOrderItemDto(
    Guid ProductId,
    int Quantity
);