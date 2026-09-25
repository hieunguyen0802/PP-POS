using MediatR;
using POS.Application.DTOs.Customer;

namespace POS.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto>;
