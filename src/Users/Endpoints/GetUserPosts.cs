namespace Chirper.Users.Endpoints;

public class GetUserPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/posts", Handle)
        .WithSummary("Get a user's posts")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<User, Request>(x => x.Id);

    public record Request(int Id, int? Page, int? PageSize) : IPagedRequest;
    public class RequestValidator : PagedRequestValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
    public record Response(int Id, string Title, string? Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, int LikesCount, int CommentsCount);

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Posts
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.Id,
                x.Title,
                x.Content,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.Likes.Count,
                x.Comments.Count
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
