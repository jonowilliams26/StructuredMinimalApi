namespace Chirper.Posts.Endpoints;

public class UpdatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/", Handle)
        .WithSummary("Updates a post")
        .WithRequest<Request>()
        .EnsureUserOwnsEntity<Post>(x => x.Id);

    public record Request(int Id, string Content);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Content).NotEmpty().MaximumLength(250);
        }
    }

    private static async Task<Ok> Handle(Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var post = await db.Posts.SingleAsync(x => x.Id == request.Id, ct);
        post.Content = request.Content;
        post.LastUpdatedAtUtc = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return TypedResults.Ok();
    }
}