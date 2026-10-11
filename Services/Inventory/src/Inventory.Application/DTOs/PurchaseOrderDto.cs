namespace Inventory.Application.DTOs;

public record PurchaseOrderItemDto(Guid ProductId, int Quantity, decimal UnitPrice);

public record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    DateTime OrderDate,
    Guid SupplierId,
    string SupplierName,
    string Status,
    decimal Total,
    List<PurchaseOrderItemDto> Items);