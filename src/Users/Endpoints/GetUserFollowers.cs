namespace Chirper.Users.Endpoints;

public class GetUserFollowers : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/followers", Handle)
        .WithSummary("Get a user's followers")
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

    public record Response(int Id, string Username, string Name, DateTime CreatedAtUtc);

    private static async Task<PagedList<Response>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        return await database.Follows
            .Where(x => x.FollowedUserId == request.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new Response
            (
                x.FollowerUser.Id,
                x.FollowerUser.Username,
                x.FollowerUser.DisplayName,
                x.CreatedAtUtc
            ))
            .ToPagedListAsync(request, cancellationToken);
    }
}
