namespace Chirper.Comments.Endpoints;

public class CreateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new comment")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Post, Request>(x => x.PostId);

    public record Request(int PostId, string Content, int? ReplyToCommentId);
    public record Response(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.PostId).GreaterThan(0);
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.ReplyToCommentId)
                .GreaterThan(0)
                .When(x => x.ReplyToCommentId.HasValue);
        }
    }

    private static async Task<Response> Handle(Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = claimsPrincipal.GetUserId(),
            Content = request.Content,
            ReplyToCommentId = request.ReplyToCommentId
        };
        await db.Comments.AddAsync(comment, ct);
        await db.SaveChangesAsync(ct);
        return new Response(comment.Id);
    }
}