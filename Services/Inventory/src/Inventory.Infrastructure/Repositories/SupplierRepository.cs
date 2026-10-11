using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly InventoryDbContext _context;

    public SupplierRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .AnyAsync(s => s.Name == name, cancellationToken);
    }



    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Suppliers.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Suppliers.ToListAsync(cancellationToken);

    public async Task AddAsync(Supplier entity, CancellationToken cancellationToken = default)
        => await _context.Suppliers.AddAsync(entity, cancellationToken);

    public void Update(Supplier entity) => _context.Suppliers.Update(entity);

    public void Delete(Supplier entity) => _context.Suppliers.Remove(entity);
}