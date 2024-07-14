namespace Chirper.Users.Endpoints;

public class GetUserLikedComments : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/liked-comments", Handle)
        .WithSummary("Get a user's liked comments")
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
        int PostId,
        int UserId,
        string Username,
        string DisplayName,
        string Content,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        int LikesCount,
        int RepliesCount,
        DateTime LikedAtUtc
    );

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.CommentLikes
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.CommentId,
                x.Comment.PostId,
                x.Comment.UserId,
                x.Comment.User.Username,
                x.Comment.User.DisplayName,
                x.Comment.Content,
                x.Comment.CreatedAtUtc,
                x.Comment.UpdatedAtUtc,
                x.Comment.Likes.Count,
                x.Comment.Replies.Count,
                x.CreatedAtUtc
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
