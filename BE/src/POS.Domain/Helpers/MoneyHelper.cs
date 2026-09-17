namespace POS.Domain.Helpers;

public static class MoneyHelper
{
    public static decimal RoundCurrency(decimal amount)
    {
        return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculateTax(decimal amount, decimal taxRate)
    {
        if (taxRate <= 0m) return 0m;
        return RoundCurrency(amount * (taxRate / 100m));
    }

}