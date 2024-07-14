namespace Chirper.Users.Endpoints;

public class UnfollowUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}/unfollow", Handle)
        .WithSummary("Unfollows a user")
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

    public static async Task<Results<Ok, ValidationError>> Handle([AsParameters] Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var userId = claimsPrincipal.GetUserId();
        if (userId == request.Id)
        {
            return new ValidationError("You cannot unfollow yourself.");
        }

        var rowsDeleted = await database.Follows
            .Where(x => x.FollowerUserId == userId && x.FollowedUserId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
        {
            return new ValidationError("You are not following this user.");
        }

        // TODO: Publish event of an unfollow to decrement the follower count

        return TypedResults.Ok();
    }
}
