namespace Chirper.Posts.Endpoints;

// TODO: Add pagination

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Gets all posts");

    public record Response(int Id, string Content, int AuthorId, string Author, DateTime CreateAtUtc, DateTime? LastUpdatedAtUtc);
    
    private static async Task<Ok<Response[]>> Handle(AppDbContext db, CancellationToken ct)
    {
        var posts = await db.Posts
            .Select(x => new Response
            (
                x.Id, 
                x.Content, 
                x.Author.Id,
                x.Author.Name,
                x.CreatedAtUtc,
                x.LastUpdatedAtUtc
            ))
            .ToArrayAsync(ct);

        return TypedResults.Ok(posts);
    }
}
