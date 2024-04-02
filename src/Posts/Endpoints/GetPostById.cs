namespace Chirper.Posts.Endpoints;

public class GetPostById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Gets a post by id")
        .WithRequestValidation<Request>();

    public record Request(int Id);
    public record Response(int Id, string Content, int UserId, string Username, string UserDisplayName, DateTime CreateAtUtc, DateTime? LastUpdatedAtUtc);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Results<Ok<Response>, NotFound>> Handle([AsParameters] Request request, AppDbContext db, CancellationToken ct)
    {
        var post = await db.Posts
            .Where(x => x.Id == request.Id)
            .Select(x => new Response
            (
                x.Id,
                x.Content,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.CreatedAtUtc,
                x.LastUpdatedAtUtc
            ))
            .SingleOrDefaultAsync(ct);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }
}