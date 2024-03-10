namespace Chirper.Data.Types;

public class Post : IOwnedEntity
{
    public int Id { get; private set; }
    public required string Content { get; set; }
    public required int UserId { get; init; }
    public required User User { get; init; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAtUtc { get; set; }
    public List<Like> Likes { get; set; } = [];
}