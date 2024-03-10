namespace Chirper.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Like>()
            .HasOne<Post>()
            .WithMany(p => p.Likes)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>()
            .HasOne<User>()
            .WithMany(u => u.LikedPosts)
            .OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }

    public async Task<User> GetUser(ClaimsPrincipal claimsPrinciple, CancellationToken ct)
    {
        var id = claimsPrinciple.GetUserId();
        return await Users.FindAsync([id], ct) ?? throw new InvalidOperationException("User not found");
    }
}