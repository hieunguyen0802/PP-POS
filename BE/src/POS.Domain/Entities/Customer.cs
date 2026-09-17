using POS.Domain.Common;

namespace POS.Domain.Entities;

public class Customer : EntityBase
{
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
}