namespace Chirper.Posts.Endpoints;

public class GetPostComments : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/comments", Handle)
        .WithSummary("Get a post's top level comments")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Post, Request>(x => x.Id);

    public record Request(int Id, int? Page, int? PageSize) : IPagedRequest;
    public class RequestValidator : PagedRequestValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
    public record Response(int Id, int UserId, string Username, string DisplayName, string Content, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc, int LikesCount, int RepliesCount);

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Comments
            .Where(x => x.PostId == request.Id && x.ReplyToCommentId == null)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.Id,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.Content,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.Likes.Count,
                x.Replies.Count
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
