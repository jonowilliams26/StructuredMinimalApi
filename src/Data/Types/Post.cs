namespace Chirper.Data.Types;

public class Post : IEntity, IOwnedEntity
{
    public int Id { get; private set; }
    public Guid ReferenceId { get; private init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Content { get; set; }
    public required int UserId { get; init; }
    public User User { get; init; } = null!;
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public List<PostLike> Likes { get; init; } = [];
    public List<Comment> Comments { get; init; } = [];
}