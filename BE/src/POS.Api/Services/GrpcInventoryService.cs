using Grpc.Core;
using Inventory.Api.GrpcContracts;
using POS.Application.Common.Interfaces;

namespace POS.Api.Services;

public class GrpcInventoryService : IInventoryService
{
    private readonly StockService.StockServiceClient _client;

    public GrpcInventoryService(StockService.StockServiceClient client) => _client = client;

    public async Task<StockReservationResult> ReserveStockAsync(
        Guid productId, int quantity, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.ReserveStockAsync(
                new ReserveStockRequest { ProductId = productId.ToString(), Quantity = quantity },
                cancellationToken: cancellationToken);

            return new StockReservationResult(true, response.RemainingQuantity);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            return new StockReservationResult(false, 0);
        }
    }

    public async Task ReleaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        await _client.ReleaseStockAsync(
            new ReleaseStockRequest { ProductId = productId.ToString(), Quantity = quantity },
            cancellationToken: cancellationToken);
    }
}