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

    public class InvalidOrderStatus : DomainException
    {
        public InvalidOrderStatus(string currentStatus, string targetStatus)
            : base($"Cannot change order status from '{currentStatus}' to '{targetStatus}'.") { }
    }

    public class InsufficientStock : DomainException
    {
        public InsufficientStock(string productName, int requestedQuantity, int availableQuantity)
            : base($"Insufficient stock for '{productName}'. Requested: {requestedQuantity}, Available: {availableQuantity}.") { }
    }
}