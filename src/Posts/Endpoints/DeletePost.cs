using Chirper.Posts.Endpoints.Filters;

namespace Chirper.Posts.Endpoints;

public class DeletePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", Handle)
        .WithSummary("Deletes a post")
        .WithRequestValidation<Request>()
        .RequiresUserOwnsPost<Request>();

    public record Request(int Id) : IRequiresUserOwnsPost;
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Ok> Handle([AsParameters] Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var post = await db.Posts.SingleAsync(x => x.Id == request.Id, ct);
        db.Posts.Remove(post);
        await db.SaveChangesAsync(ct);
        return TypedResults.Ok();
    }
}