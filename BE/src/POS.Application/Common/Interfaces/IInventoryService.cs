namespace POS.Application.Common.Interfaces;

public interface IInventoryService
{
    Task<StockReservationResult> ReserveStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    Task ReleaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
}

public record StockReservationResult(bool Success, int RemainingQuantity);