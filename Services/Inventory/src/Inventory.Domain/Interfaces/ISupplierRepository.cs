using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}