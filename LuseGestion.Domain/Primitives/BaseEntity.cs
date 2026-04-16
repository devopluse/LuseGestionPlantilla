namespace LuseGestion.Domain.Primitives;

public abstract class BaseEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
