namespace Chirper.Posts.Endpoints;

public class UnlikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}/unlike", Handle)
        .WithSummary("Unlikes a post")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Post, Request>(x => x.Id);

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        var rowsDeleted = await database.PostLikes
            .Where(x => x.PostId == request.Id && x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return rowsDeleted == 0
            ? TypedResults.NotFound()
            : TypedResults.Ok();
    }
}