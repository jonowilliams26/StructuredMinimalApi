namespace Chirper.Data.Types;

public class User : IEntity
{
    public int Id { get; private init; }
    public Guid ReferenceId { get; private init; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public List<Post> Posts { get; init; } = [];
    public List<PostLike> LikedPosts { get; init; } = [];
    public List<Comment> Comments { get; init; } = [];
    public List<CommentLike> LikedComments { get; init; } = [];
    public List<Follow> Following { get; init; } = [];
    public List<Follow> Followers { get; init; } = [];
}

public class Follow
{
    public required int FollowerUserId { get; init; }
    public User FollowerUser { get; init; } = null!;

    public required int FollowedUserId { get; init; }
    public User FollowedUser { get; init; } = null!;

    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}