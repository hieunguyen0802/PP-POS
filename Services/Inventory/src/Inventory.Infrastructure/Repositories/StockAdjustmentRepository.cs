using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class StockAdjustmentRepository : IStockAdjustmentRepository
{
    private readonly InventoryDbContext _context;

    public StockAdjustmentRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<StockAdjustment>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.StockAdjustments
            .AsNoTracking()
            .Where(s => s.ProductId == productId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockAdjustment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.StockAdjustments.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<StockAdjustment>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.StockAdjustments.ToListAsync(cancellationToken);

    public async Task AddAsync(StockAdjustment entity, CancellationToken cancellationToken = default)
        => await _context.StockAdjustments.AddAsync(entity, cancellationToken);

    public void Update(StockAdjustment entity) => _context.StockAdjustments.Update(entity);

    public void Delete(StockAdjustment entity) => _context.StockAdjustments.Remove(entity);
}