using Inventory.Domain.Interfaces;
using Inventory.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;

    public UnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainExceptions.ConcurrencyConflict();
        }
    }
}