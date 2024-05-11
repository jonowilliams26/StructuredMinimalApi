
namespace Chirper.Users.Endpoints;

public class GetUserFollowers : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/followers", Handle)
        .WithSummary("Get a user's followers")
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

    public record Response(int Id, string Username, string Name, DateTime CreatedAtUtc);

    private static async Task<Response[]> Handle([AsParameters] Request request, AppDbContext db, CancellationToken ct)
    {
        return await db.Follows
            .Where(x => x.FollowedUserId == request.Id)
            .Select(x => new Response
            (
                x.FollowerUser.Id,
                x.FollowerUser.Username,
                x.FollowerUser.DisplayName,
                x.FollowerUser.CreatedAtUtc
            ))
            .ToArrayAsync(ct);
    }
}
