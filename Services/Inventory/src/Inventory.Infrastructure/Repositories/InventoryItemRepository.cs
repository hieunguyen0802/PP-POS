using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly InventoryDbContext _context;

    public InventoryItemRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.FirstOrDefaultAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<bool> ExistsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.AnyAsync(i => i.ProductId == productId, cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InventoryItems.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        await _context.InventoryItems.AddAsync(inventoryItem, cancellationToken);
    }

    public void Update(InventoryItem inventoryItem)
    {
        _context.InventoryItems.Update(inventoryItem);
    }

    public void Delete(InventoryItem inventoryItem)
    {
        _context.InventoryItems.Remove(inventoryItem);
    }
}