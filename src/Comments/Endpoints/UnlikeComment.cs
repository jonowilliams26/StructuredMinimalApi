namespace Chirper.Comments.Endpoints;

public class UnlikeComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}/unlike", Handle)
        .WithSummary("Unlike a comment")
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

    public static async Task<Results<Ok, NotFound>> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        
        var rowsDeleted = await database.CommentLikes
            .Where(x => x.CommentId == request.Id && x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
        {
            return TypedResults.NotFound();
        }

        // TODO: Publish event to notify comment unliked

        return TypedResults.Ok();
    }
}
