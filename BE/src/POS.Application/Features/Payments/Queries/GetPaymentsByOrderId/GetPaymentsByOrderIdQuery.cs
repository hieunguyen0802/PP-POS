using MediatR;
using POS.Application.DTOs.Payment;

namespace POS.Application.Features.Payments.Queries.GetPaymentsByOrderId;

public record GetPaymentsByOrderIdQuery(Guid OrderId) : IRequest<List<PaymentDto>>;
