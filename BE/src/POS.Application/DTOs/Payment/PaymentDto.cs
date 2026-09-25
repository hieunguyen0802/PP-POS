namespace POS.Application.DTOs.Payment;

public record PaymentDto(
    Guid Id,
    string ReferenceNumber,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    Guid OrderId
);