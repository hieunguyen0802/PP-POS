namespace POS.Domain.Common;

public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System"; // Stores UserId or Username

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}