using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;
using POS.Domain.Entities;
using POS.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace POS.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryService _inventoryService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IInventoryService inventoryService,
        ICodeGenerator codeGenerator,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _inventoryService = inventoryService;
        _codeGenerator = codeGenerator;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = await _codeGenerator.GenerateCodeAsync("ORD", cancellationToken);
        var order = new Order(request.CustomerId, orderNumber);
        var reserved = new List<(Guid ProductId, int Quantity)>();

        try
        {
            foreach (var itemDto in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
                if (product == null)
                    throw new DomainExceptions.EntityNotFound("Product", itemDto.ProductId);

                var reservation = await _inventoryService.ReserveStockAsync(product.Id, itemDto.Quantity, cancellationToken);
                if (!reservation.Success)
                    throw new DomainExceptions.InsufficientStock(product.ProductName, itemDto.Quantity);

                reserved.Add((product.Id, itemDto.Quantity));
                order.AddItem(product.Id, itemDto.Quantity, product.UnitPrice);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
        catch
        {
            foreach (var (productId, quantity) in reserved)
            {
                try
                {
                    // CancellationToken.None: compensation must still run if the request was cancelled
                    await _inventoryService.ReleaseStockAsync(productId, quantity, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to release {Quantity} of product {ProductId} after a failed order.", quantity, productId);
                }
            }
            throw;
        }
    }
}