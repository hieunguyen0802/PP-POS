using FluentValidation;
namespace Inventory.Application.Features.PurchaseOrders.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandValidator : AbstractValidator<ReceivePurchaseOrderCommand>
{
    public ReceivePurchaseOrderCommandValidator() => RuleFor(x => x.PurchaseOrderId).NotEmpty();
}