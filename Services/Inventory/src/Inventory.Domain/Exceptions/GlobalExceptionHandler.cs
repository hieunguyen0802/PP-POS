namespace Inventory.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}

public static class DomainExceptions
{
    public class EntityNotFound : DomainException
    {
        public EntityNotFound(string entityName, object key)
            : base($"Entity '{entityName}' with key '{key}' was not found.") { }
    }

    public class EntityAlreadyExists : DomainException
    {
        public EntityAlreadyExists(string entityName, object key)
            : base($"Entity '{entityName}' with key '{key}' already exists.") { }
    }

    public class InsufficientStock : DomainException
    {
        public InsufficientStock(Guid productId, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for product '{productId}'. Requested: {requestedQuantity}, Available: {availableQuantity}.") { }
    }

    public class InvalidPurchaseOrderStatus : DomainException
    {
        public InvalidPurchaseOrderStatus(string currentStatus, string targetStatus)
            : base($"Cannot change purchase order status from '{currentStatus}' to '{targetStatus}'.") { }
    }

    public class ConcurrencyConflict : DomainException
    {
        public ConcurrencyConflict()
            : base("The record was modified by another request. Please retry.") { }
    }
}