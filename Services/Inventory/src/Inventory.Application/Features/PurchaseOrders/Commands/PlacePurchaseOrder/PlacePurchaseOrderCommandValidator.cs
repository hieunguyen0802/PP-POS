using FluentValidation;
namespace Inventory.Application.Features.PurchaseOrders.Commands.PlacePurchaseOrder;

public class PlacePurchaseOrderCommandValidator : AbstractValidator<PlacePurchaseOrderCommand>
{
    public PlacePurchaseOrderCommandValidator() => RuleFor(x => x.PurchaseOrderId).NotEmpty();
}