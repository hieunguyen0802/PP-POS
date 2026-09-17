using POS.Domain.Common;

namespace POS.Domain.Entities;

public class Payment : EntityBase
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0m;
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public PaymentMethod PaymentMethod { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
}