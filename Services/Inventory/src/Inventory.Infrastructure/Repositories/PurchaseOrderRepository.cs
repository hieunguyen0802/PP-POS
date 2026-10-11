using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly InventoryDbContext _context;

    public PurchaseOrderRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    => await _context.PurchaseOrders
        .Include(p => p.Supplier)
        .Include(p => p.Items)
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);



    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.PurchaseOrders.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<PurchaseOrder>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.PurchaseOrders.ToListAsync(cancellationToken);

    public async Task AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
        => await _context.PurchaseOrders.AddAsync(entity, cancellationToken);

    public void Update(PurchaseOrder entity) => _context.PurchaseOrders.Update(entity);

    public void Delete(PurchaseOrder entity) => _context.PurchaseOrders.Remove(entity);
}