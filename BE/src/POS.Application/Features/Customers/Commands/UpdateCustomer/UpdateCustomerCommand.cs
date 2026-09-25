using MediatR;

namespace POS.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(Guid CustomerId, string CustomerName, string CustomerPhone, string CustomerEmail, string? CustomerAddress) : IRequest<Unit>;
