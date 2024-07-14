namespace Chirper.Comments.Endpoints;

public class CreateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new comment")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Post, Request>(x => x.PostId)
        .WithEnsureEntityExists<Comment, Request>(x => x.ReplyToCommentId);

    public record Request(int PostId, string Content, int? ReplyToCommentId);
    public record Response(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.PostId).GreaterThan(0);
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.ReplyToCommentId).GreaterThan(0);
        }
    }

    private static async Task<Ok<Response>> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = claimsPrincipal.GetUserId(),
            Content = request.Content,
            ReplyToCommentId = request.ReplyToCommentId
        };
        await database.Comments.AddAsync(comment, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new Response(comment.Id);
        return TypedResults.Ok(response);
    }
}