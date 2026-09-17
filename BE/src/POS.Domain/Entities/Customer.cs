using POS.Domain.Common;
using POS.Domain.Helpers;

namespace POS.Domain.Entities;

public class Customer : EntityBase
{
    public string CustomerCode { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string? CustomerAddress { get; private set; }

    private Customer() { } // For EF Core

    public Customer(string customerName, string customerPhone, string customerEmail, string? customerAddress = null)
    {

        ValidateCustomers(customerName, customerPhone, customerEmail);

        CustomerCode = CodeGenerator.GenerateCustomerNumber();
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

    private void ValidateCustomers(string customerName, string customerPhone, string customerEmail)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty.", nameof(customerName));

        if (string.IsNullOrWhiteSpace(customerPhone))
            throw new ArgumentException("Customer phone cannot be empty.", nameof(customerPhone));

        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email cannot be empty.", nameof(customerEmail));
    }

}