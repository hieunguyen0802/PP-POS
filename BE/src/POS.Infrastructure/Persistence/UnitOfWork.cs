using POS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace POS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PosDBContext _context;

    public UnitOfWork(PosDBContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}