using MediatR;
using POS.Application.DTOs.Payment;

namespace POS.Application.Features.Payments.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto>;
