namespace Chirper.Posts.Endpoints;

public class LikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", Handle)
        .WithSummary("Likes a post")
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
        var doesLikeExist = await db.Likes.AnyAsync(x => x.PostId == request.Id && x.UserId == userId, ct);
        if (doesLikeExist)
        {
            return TypedResults.Ok();
        }

        var like = new Like
        {
            PostId = request.Id,
            UserId = userId
        };
        await db.Likes.AddAsync(like, ct);
        await db.SaveChangesAsync(ct);
        return TypedResults.Ok();
    }
}