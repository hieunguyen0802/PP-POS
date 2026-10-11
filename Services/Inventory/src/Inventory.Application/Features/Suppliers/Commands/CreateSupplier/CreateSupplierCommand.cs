using MediatR;

namespace Inventory.Application.Features.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(string Name, string ContactInfo) : IRequest<Guid>;