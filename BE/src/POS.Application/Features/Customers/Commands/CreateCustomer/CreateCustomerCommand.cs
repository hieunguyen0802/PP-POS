using MediatR;

namespace POS.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(string CustomerName, string CustomerPhone, string CustomerEmail, string? CustomerAddress) : IRequest<Guid>;
