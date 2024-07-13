namespace Chirper.Comments.Endpoints;

public class LikeComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", Handle)
        .WithSummary("Like a comment")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Comment, Request>(x => x.Id);

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public static async Task<Ok> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var exists = await database.CommentLikes
            .AnyAsync(x => x.CommentId == request.Id && x.UserId == userId, cancellationToken);

        if (exists)
        {
            return TypedResults.Ok();
        }

        var like = new CommentLike
        {
            CommentId = request.Id,
            UserId = userId,
        };

        await database.CommentLikes.AddAsync(like, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
