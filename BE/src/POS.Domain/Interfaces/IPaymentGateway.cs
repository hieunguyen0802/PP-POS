
namespace POS.Domain.Interfaces;

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(decimal amount, PaymentMethod method, string reference, CancellationToken cancellationToken = default);
}