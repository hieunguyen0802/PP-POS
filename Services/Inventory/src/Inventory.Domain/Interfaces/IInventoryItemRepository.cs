using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IInventoryItemRepository : IRepository<InventoryItem>
{
    Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}