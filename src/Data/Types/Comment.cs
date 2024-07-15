namespace Chirper.Data.Types;

public class Comment : IEntity, IOwnedEntity
{
    public int Id { get; private set; }
    public Guid ReferenceId { get; private init; } = Guid.NewGuid();
    public required int PostId { get; init; }
    public Post Post { get; init; } = null!;
    public required int UserId { get; init; }
    public User User { get; init; } = null!;
    public required string Content { get; set; }
    public int? ReplyToCommentId { get; init; }
    public Comment? ReplyToComment { get; init; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public List<Comment> Replies { get; init; } = [];
    public List<CommentLike> Likes { get; init; } = [];
}

public class CommentLike : IOwnedEntity
{
    public required int CommentId { get; init; }
    public Comment Comment { get; init; } = null!;

    public required int UserId { get; init; }
    public User User { get; init; } = null!;

    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}