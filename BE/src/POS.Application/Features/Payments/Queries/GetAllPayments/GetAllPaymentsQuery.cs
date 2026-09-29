using MediatR;
using POS.Application.DTOs.Payment;

namespace POS.Application.Features.Payments.Queries.GetAllPayments;

public record GetAllPaymentsQuery() : IRequest<List<PaymentDto>>;
