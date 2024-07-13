namespace Chirper.Data.Types;

public class PostLike : IOwnedEntity
{
    public required int PostId { get; init; }
    public Post Post { get; init; } = null!;
    public required int UserId { get; init; }
    public User User { get; init; } = null!;
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}