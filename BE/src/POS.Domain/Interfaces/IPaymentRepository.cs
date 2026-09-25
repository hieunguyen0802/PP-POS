using POS.Domain.Entities;

namespace POS.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
