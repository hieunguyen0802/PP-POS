using MediatR;
using POS.Domain.Entities;

namespace POS.Application.Features.Payments.Commands.CreatePayment;

public record CreatePaymentCommand(Guid OrderId, PaymentMethod PaymentMethod, decimal Amount) : IRequest<Guid>;
