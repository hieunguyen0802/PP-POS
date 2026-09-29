namespace POS.Application.DTOs.Payment;

public record PaymentDto(
    Guid Id,
    string ReferenceNumber,
    decimal AmountTendered,
    decimal AmountApplied,
    decimal ChangeGiven,
    DateTime PaymentDate,
    string PaymentMethod,
    Guid OrderId
);
