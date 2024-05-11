namespace Chirper.Data.Types;

public class User : IEntity
{
    public int Id { get; private init; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public List<Post> Posts { get; set; } = [];
    public List<Like> LikedPosts { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<Follow> Following { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];
}

public class Follow
{
    public int FollowerUserId { get; set; }
    public User FollowerUser { get; set; } = null!;

    public int FollowedUserId { get; set; }
    public User FollowedUser { get; set; } = null!;

    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
}