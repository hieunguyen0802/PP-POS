using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;

namespace POS.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository

{
    private readonly PosDBContext _context;
    public OrderRepository(PosDBContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders.ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
        .Where(o => o.CustomerId == customerId)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(entity, cancellationToken);
    }

    public void Update(Order entity)
    {
        _context.Orders.Update(entity);
    }


    public void Delete(Order entity)
    {
        _context.Orders.Remove(entity);
    }


}