namespace Chirper.Users.Endpoints;

public class GetUserComments : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/comments", Handle)
        .WithSummary("Get a user's comments")
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
        string Content,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        int LikesCount,
        int RepliesCount
    );

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Comments
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.Id,
                x.PostId,
                x.Content,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.Likes.Count,
                x.Replies.Count
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
