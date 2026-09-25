using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;

namespace POS.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly PosDBContext _context;

    public PaymentRepository(PosDBContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Payments.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments.Where(p => p.OrderId == orderId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(entity, cancellationToken);
    }

    public void Update(Payment entity)
    {
        _context.Payments.Update(entity);
    }

    public void Delete(Payment entity)
    {
        _context.Payments.Remove(entity);
    }
}
