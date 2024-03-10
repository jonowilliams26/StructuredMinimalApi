namespace Chirper.Data.Types;

public class Like
{
    public int Id { get; private set; }
    public required int PostId { get; init; }
    public required int UserId { get; init; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}