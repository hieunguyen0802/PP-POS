namespace POS.Application.DTOs.Product;

public record ProductDto(
    Guid Id,
    string ProductCode,
    string ProductName,
    decimal UnitPrice,
    string ProductBarcode,
    bool IsActive
);