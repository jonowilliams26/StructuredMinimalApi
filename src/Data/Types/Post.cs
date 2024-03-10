namespace Chirper.Data.Types;

public class Post
{
    public int Id { get; private set; }
    public required string Content { get; set; }
    public required User Author { get; init; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAtUtc { get; set; }
    public List<Like> Likes { get; set; } = [];
}