namespace Chirper.Users.Endpoints;

public class GetUserFollowing : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/following", Handle)
        .WithSummary("Get a user's following list")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<User, Request>(x => x.Id);

    public record Request(int Id, int? Page, int? PageSize) : IPagedRequest;
    public class RequestValidator : PagedRequestValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
    public record Response(int Id, string Username, string DisplayName, DateTime CreatedAtUtc);

    public static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Follows
            .Where(x => x.FollowerUserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.FollowedUserId,
                x.FollowedUser.Username,
                x.FollowedUser.DisplayName,
                x.CreatedAtUtc
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
