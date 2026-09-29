using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Infrastructure.Persistence.Services;

public class CodeGeneratorService : ICodeGenerator
{
    private const int MaxAttempts = 5;

    private readonly PosDBContext _context;

    public CodeGeneratorService(PosDBContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateCodeAsync(string category, CancellationToken cancellationToken = default)
    {
        var dateKey = DateTime.UtcNow.ToString("yyyyMMdd");

        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var nextValue = await TryIncrementExistingAsync(category, dateKey, cancellationToken)
                ?? await TryCreateFirstAsync(category, dateKey, cancellationToken);

            if (nextValue is not null)
            {
                return $"{category}-{dateKey}-{nextValue:D4}";
            }


        }

        throw new InvalidOperationException(
            $"Could not generate a unique code for category '{category}' after {MaxAttempts} attempts.");
    }


    private async Task<int?> TryIncrementExistingAsync(string category, string dateKey, CancellationToken cancellationToken)
    {
        var result = await _context.Database
            .SqlQuery<int>($@"UPDATE CodeSequences
                               SET LastValue = LastValue + 1
                               OUTPUT INSERTED.LastValue
                               WHERE Category = {category} AND DateKey = {dateKey}")
            .ToListAsync(cancellationToken);

        return result.Count > 0 ? result[0] : null;
    }


    private async Task<int?> TryCreateFirstAsync(string category, string dateKey, CancellationToken cancellationToken)
    {
        try
        {
            var sequence = new CodeSequence(category, dateKey, 1);
            _context.CodeSequences.Add(sequence);
            await _context.SaveChangesAsync(cancellationToken);
            return 1;
        }
        catch (DbUpdateException)
        {
            foreach (var entry in _context.ChangeTracker.Entries<CodeSequence>().ToList())
            {
                entry.State = EntityState.Detached;
            }

            return null;
        }
    }
}
