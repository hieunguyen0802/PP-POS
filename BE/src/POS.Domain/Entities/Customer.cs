using POS.Domain.Common;

namespace POS.Domain.Entities;

public class Customer : AggregateRoot
{
    public string CustomerCode { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string? CustomerAddress { get; private set; }

    public string KeycloakId { get; private set; } = string.Empty;


    private Customer() { } // For EF Core

    public Customer(string customerName, string customerPhone, string customerEmail, string customerCode, string? customerAddress = null)
    {

        ValidateCustomers(customerName, customerPhone, customerEmail, customerCode);

        CustomerCode = customerCode;
        CustomerName = customerName;
        CustomerPhone = customerPhone;
        CustomerEmail = customerEmail;
        CustomerAddress = customerAddress;
    }

    public void UpdateCustomer(string customerName, string customerPhone, string customerEmail, string? customerAddress = null)
    {
        ValidateCustomers(customerName, customerPhone, customerEmail);

        CustomerName = customerName;
        CustomerPhone = customerPhone;
        CustomerEmail = customerEmail;
        CustomerAddress = customerAddress;
    }

    private void ValidateCustomers(string customerName, string customerPhone, string customerEmail, string? customerCode = null)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty.", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone cannot be empty.", nameof(customerPhone));

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email cannot be empty.", nameof(customerEmail));

        if (string.IsNullOrWhiteSpace(customerCode))
            throw new ArgumentException("Customer code cannot be empty.", nameof(customerCode));
    }

}
