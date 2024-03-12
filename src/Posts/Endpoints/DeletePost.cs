namespace Chirper.Posts.Endpoints;

public class DeletePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", Handle)
        .WithSummary("Deletes a post")
        .WithRequestValidation<Request>()
        .EnsureUserOwnsEntity<Post>(x => x.Id);

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
        await db.Posts
            .Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync(ct);

        return TypedResults.Ok();
    }
}