namespace Chirper.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUsersTable(modelBuilder);
        ConfigurePostsTable(modelBuilder);
        ConfigureCommentsTable(modelBuilder);
        ConfigureLikesTable(modelBuilder);
        ConfigureFollowsTable(modelBuilder);
        ConfigureCommentLikesTable(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUsersTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<User>();

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();
        
        builder.HasMany(x => x.Posts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.LikedPosts)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.LikedComments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Following)
            .WithOne(x => x.FollowerUser)
            .HasForeignKey(x => x.FollowerUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Followers)
            .WithOne(x => x.FollowedUser)
            .HasForeignKey(x => x.FollowedUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigurePostsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Post>();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Post)
            .HasForeignKey(x => x.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureCommentsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Comment>();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Comment)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Replies)
            .WithOne(x => x.ReplyToComment)
            .HasForeignKey(x => x.ReplyToCommentId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureLikesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<PostLike>();
        builder.HasKey(x => new { x.PostId, x.UserId });
    }

    private static void ConfigureFollowsTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<Follow>();
        builder.HasKey(x => new { x.FollowerUserId, x.FollowedUserId });
    }

    private static void ConfigureCommentLikesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<CommentLike>();
        builder.HasKey(x => new { x.CommentId, x.UserId });
    }
}