namespace POS.Domain.Entities;

// Tracks the last sequence number issued for a given (Category, DateKey) pair
// (e.g. "CUST" / "20260928"), so ICodeGenerator can atomically hand out the
// next number for that category on that day.
public class CodeSequence
{
    public string Category { get; private set; } = string.Empty;
    public string DateKey { get; private set; } = string.Empty;
    public int LastValue { get; private set; }

    private CodeSequence() { } // For EF Core

    public CodeSequence(string category, string dateKey, int lastValue = 1)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty.", nameof(category));

        if (string.IsNullOrWhiteSpace(dateKey))
            throw new ArgumentException("Date key cannot be empty.", nameof(dateKey));

        Category = category;
        DateKey = dateKey;
        LastValue = lastValue;
    }

    public void Increment() => LastValue++;
}
