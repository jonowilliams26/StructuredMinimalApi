namespace Chirper.Posts.Endpoints;

// TODO: Add pagination

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Gets all posts");

    public record Response
    (
        int Id,
        string Content,
        int UserId,
        string Username,
        string UserDisplayName,
        DateTime CreateAtUtc,
        DateTime? LastUpdatedAtUtc,
        int NumberOfLikes,
        int NumberOfComments
    );

    private static async Task<Ok<Response[]>> Handle(AppDbContext db, CancellationToken ct)
    {
        var posts = await db.Posts
            .Select(x => new Response
            (
                x.Id,
                x.Content,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.CreatedAtUtc,
                x.LastUpdatedAtUtc,
                x.Likes.Count,
                x.Comments.Count
            ))
            .ToArrayAsync(ct);

        return TypedResults.Ok(posts);
    }
}
