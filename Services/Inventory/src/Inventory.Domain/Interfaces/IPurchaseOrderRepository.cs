using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<PurchaseOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

}