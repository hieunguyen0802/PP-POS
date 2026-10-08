using Inventory.Application.Features.InventoryItems.Commands.ReleaseStock;
using FluentValidation;
using Grpc.Core;
using Inventory.Api.GrpcContracts;
using Inventory.Application.Features.InventoryItems.Commands.ReserveStock;
using Inventory.Domain.Exceptions;
using MediatR;


namespace Inventory.Api.GrpcServices;

public class StockGrpcService : StockService.StockServiceBase
{
    private readonly IMediator _mediator;

    public StockGrpcService(IMediator mediator) => _mediator = mediator;


    public override async Task<ReserveStockResponse> ReserveStock(ReserveStockRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "product_id must be a valid GUID"));

        try
        {
            var remaining = await _mediator.Send(
                new ReserveStockCommand(productId, request.Quantity), context.CancellationToken);

            return new ReserveStockResponse { RemainingQuantity = remaining };
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (DomainExceptions.InsufficientStock ex)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        }
        catch (DomainExceptions.ConcurrencyConflict ex)
        {
            // Aborted = "retry the whole operation"; same meaning as our HTTP 409
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }

    public override async Task<ReleaseStockResponse> ReleaseStock(ReleaseStockRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProductId, out var productId))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "product_id must be a valid GUID"));

        try
        {
            var remaining = await _mediator.Send(new ReleaseStockCommand(productId, request.Quantity), context.CancellationToken);
            return new ReleaseStockResponse { RemainingQuantity = remaining };
        }
        catch (ValidationException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (DomainExceptions.EntityNotFound ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (DomainExceptions.ConcurrencyConflict ex)
        {
            throw new RpcException(new Status(StatusCode.Aborted, ex.Message));
        }
    }
}