namespace POS.Application.DTOs.Order;

public record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid? CustomerId,
    string Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);