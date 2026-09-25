using MediatR;
using POS.Application.DTOs.Customer;

namespace POS.Application.Features.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery() : IRequest<List<CustomerDto>>;
