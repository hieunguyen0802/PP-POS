using POS.Domain.Entities;
namespace POS.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>

{
    Task<Order?> GetWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}