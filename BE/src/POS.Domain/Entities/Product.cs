using POS.Domain.Common;
using POS.Domain.Events;
using POS.Domain.Exceptions;
using POS.Domain.Helpers;

namespace POS.Domain.Entities;

public class Product : AggregateRoot
{
    public string ProductCode { get; private set; } = string.Empty;
    public string ProductBarcode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; } = 0m;
    public bool IsActive { get; private set; } = true;

    private Product() { } // For EF Core

    public Product(string productCode, string productName, decimal unitPrice, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code cannot be empty.", nameof(productCode));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        ProductCode = productCode;
        ProductName = productName;
        UnitPrice = unitPrice;
        ProductBarcode = barcode ?? string.Empty;
    }

    public void UpdateProduct(string productName, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name cannot be empty.", nameof(productName));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));

        ProductName = productName;
        UnitPrice = unitPrice;
    }

    public void ChangeActiveStatus()
    {
        IsActive = !IsActive;

    }

}