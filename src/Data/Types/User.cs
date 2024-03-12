namespace Chirper.Data.Types;

public class User
{
    public int Id { get; private init; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAtUtc { get; private init; } = DateTime.UtcNow;
    public List<Post> Posts { get; set; } = [];
    public List<Like> LikedPosts { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}
