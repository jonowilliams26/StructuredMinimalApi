namespace Chirper.Users.Endpoints;

public class FollowUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/follow", Handle)
        .WithSummary("Follows a user")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<User, Request>(x => x.Id);

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Results<Ok, ValidationError>> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();

        if (userId == request.Id)
        {
            return new ValidationError("You cannot follow yourself.");
        }

        var isAlreadyFollowing = await database.Follows.AnyAsync(x => 
            x.FollowerUserId == userId && 
            x.FollowedUserId == request.Id,
            cancellationToken
        );

        if (isAlreadyFollowing)
        {
            return new ValidationError("You are already following this user.");
        }

        var follow = new Follow
        {
            FollowerUserId = userId,
            FollowedUserId = request.Id
        };

        // TODO: Send a notification to the user being followed

        await database.Follows.AddAsync(follow, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
