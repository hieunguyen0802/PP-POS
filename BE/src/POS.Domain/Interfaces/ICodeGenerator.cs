namespace POS.Domain.Interfaces;

public interface ICodeGenerator
{
    Task<string> GenerateCodeAsync(string category, CancellationToken cancellationToken = default);
}
