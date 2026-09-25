namespace POS.Application.DTOs.Customer;

public record CustomerDto(
    Guid Id,
    string CustomerCode,
    string CustomerName,
    string CustomerPhone,
    string CustomerEmail,
    string? CustomerAddress
);