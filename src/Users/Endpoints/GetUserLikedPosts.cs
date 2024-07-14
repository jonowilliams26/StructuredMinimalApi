namespace Chirper.Users.Endpoints;

public class GetUserLikedPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/liked-posts", Handle)
        .WithSummary("Get a user's liked posts")
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
    public record Response(
        int Id,
        int UserId,
        string? Content,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        int LikesCount,
        int CommentsCount,
        DateTime LikedAtUtc
    );

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.PostLikes
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.PostId,
                x.Post.UserId,
                x.Post.Content,
                x.Post.CreatedAtUtc,
                x.Post.UpdatedAtUtc,
                x.Post.Likes.Count,
                x.Post.Comments.Count,
                x.CreatedAtUtc
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
