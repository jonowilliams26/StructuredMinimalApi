namespace Chirper.Posts.Endpoints;

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", Handle)
        .WithSummary("Gets all posts")
        .WithRequestValidation<Request>();

    public record Request(int? Page, int? PageSize) : IPagedRequest;
    public class RequestValidator : PagedRequestValidator<Request>;
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

    private static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        var posts = await database.Posts
            .OrderByDescending(x => x.CreatedAtUtc)
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
            .ToPagedListAsync(request, cancellationToken);

        return posts;
    }
}
