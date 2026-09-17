namespace POS.Domain.Helpers;

public static class CodeGenerator
{
    public static string GenerateOrderNumber()
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        string randomPart = Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
        return $"ORD-{datePart}-{randomPart}";
    }

    public static string GenerateCustomerNumber()
    {
        return $"CUST-{Random.Shared.Next(10000, 99999)}"; ;
    }

    public static string GenerateProductCode(string productType)
    {
        return $"{productType}-{Random.Shared.Next(10000, 99999)}"; ;
    }

    public static string GenerateReferenceNumber()
    {
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"REF-{datePart}-{Random.Shared.Next(10000, 99999)}"; ;
    }
}