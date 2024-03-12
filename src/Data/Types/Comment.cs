namespace Chirper.Data.Types;

public class Comment : IOwnedEntity
{
    public int Id { get; private set; }
    public required int PostId { get; init; }
    public required int UserId { get; init; }
    public required string Content { get; set; }
    public int? ReplyToCommentId { get; init; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public List<Comment> Replies { get; set; } = [];
}