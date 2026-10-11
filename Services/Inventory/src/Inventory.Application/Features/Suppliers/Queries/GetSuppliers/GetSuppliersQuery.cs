using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Features.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery() : IRequest<IReadOnlyList<SupplierDto>>;