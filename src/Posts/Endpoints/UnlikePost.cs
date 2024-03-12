namespace Chirper.Posts.Endpoints;

public class UnlikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}/unlike", Handle)
        .WithSummary("Unlikes a post")
        .WithRequestValidation<Request>()
        .EnsureEntityExists<Post>(x => x.Id);

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Ok> Handle([AsParameters] Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var userId = claimsPrincipal.GetUserId();
        await db.Likes
            .Where(x => x.PostId == request.Id && x.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return TypedResults.Ok();
    }
}