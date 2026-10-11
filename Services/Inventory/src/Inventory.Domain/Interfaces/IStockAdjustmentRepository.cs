using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IStockAdjustmentRepository : IRepository<StockAdjustment>
{
    Task<IReadOnlyList<StockAdjustment>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}