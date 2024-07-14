namespace Chirper.Posts.Endpoints;

public class LikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", Handle)
        .WithSummary("Likes a post")
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

    private static async Task<Ok> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        var doesLikeExist = await database.PostLikes.AnyAsync(x => x.PostId == request.Id && x.UserId == userId, cancellationToken);
        if (doesLikeExist)
        {
            return TypedResults.Ok();
        }

        var like = new PostLike
        {
            PostId = request.Id,
            UserId = userId
        };
        await database.PostLikes.AddAsync(like, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}