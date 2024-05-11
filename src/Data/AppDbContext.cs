namespace Chirper.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsersTable(modelBuilder);
        ConfigureCommentsTable(modelBuilder);
        ConfigureLikesTable(modelBuilder);
        ConfigureFollowsTable(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUsersTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();
        builder.HasMany(x => x.Following)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Followers)
            .WithOne()
            .HasForeignKey(x => x.FollowingUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureCommentsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Comment>();
        builder.HasOne<Comment>()
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ReplyToCommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Post>()
            .WithMany(p => p.Comments)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureLikesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Like>();
        builder.HasOne<Post>()
            .WithMany(p => p.Likes)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<User>()
            .WithMany(u => u.LikedPosts)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureFollowsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Follow>();
        builder.HasKey(x => new { x.UserId, x.FollowingUserId });
    }
}