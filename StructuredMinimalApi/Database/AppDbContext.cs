namespace StructuredMinimalApi.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Post> Posts { get; set; }
}