namespace Chirper.Comments.Endpoints;

public class GetCommentReplies : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/replies", Handle)
        .WithSummary("Gets all replies to a comment")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Comment, Request>(x => x.Id);

    public record Request(int Id, int? Page, int? PageSize) : IPagedRequest;
    public record Response(int Id, int UserId, string Username, string UserDisplayName, string Content, int NumberOfReplies);
    public class RequestValidator : PagedRequestValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        var replies = await database.Comments
            .Where(x => x.ReplyToCommentId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.Id,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.Content,
                x.Replies.Count
            ))
            .ToPagedListAsync(request, cancellationToken);

        return replies;
    }
}