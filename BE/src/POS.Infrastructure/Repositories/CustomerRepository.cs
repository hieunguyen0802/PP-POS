using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;

namespace POS.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository

{
    private readonly PosDBContext _context;
    public CustomerRepository(PosDBContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerCode == customerCode, cancellationToken);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerPhone == phone, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == email, cancellationToken);
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.AnyAsync(c => c.CustomerPhone == phone, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> SearchByNameOrPhoneAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.CustomerName.Contains(searchTerm) || c.CustomerPhone.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
    }


    public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(entity, cancellationToken);
    }

    public void Update(Customer entity)
    {
        _context.Customers.Update(entity);
    }


    public void Delete(Customer entity)
    {
        _context.Customers.Remove(entity);
    }


}