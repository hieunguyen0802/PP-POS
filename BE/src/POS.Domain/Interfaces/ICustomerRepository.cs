using POS.Domain.Entities;

namespace POS.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default);
    Task<Customer?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> SearchByNameOrPhoneAsync(string searchTerm, CancellationToken cancellationToken = default);
}