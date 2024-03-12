namespace Chirper.Comments.Endpoints;

// TODO: Add Pagination

public class GetCommentReplies : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/replies", Handle)
        .WithSummary("Gets all replies to a comment")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Comment, Request>(x => x.Id);

    public record Request(int Id);
    public record Response(int Id, string Username, string UserDisplayName, string Content, int NumberOfReplies);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Ok<Response[]>> Handle([AsParameters] Request request, AppDbContext db, CancellationToken ct)
    {
        var replies = await db.Comments
            .Where(x => x.ReplyToCommentId == request.Id)
            .Select(x => new Response
            (
                x.Id,
                x.User.Username,
                x.User.Name,
                x.Content,
                x.Replies.Count
            ))
            .ToArrayAsync(ct);

        return TypedResults.Ok(replies);
    }
}