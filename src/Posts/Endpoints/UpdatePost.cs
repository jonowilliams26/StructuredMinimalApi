namespace Chirper.Posts.Endpoints;

public class UpdatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/", Handle)
        .WithSummary("Updates a post")
        .WithRequestValidation<Request>()
        .WithEnsureUserOwnsEntity<Post, Request>(x => x.Id);

    public record Request(int Id, string Title, string? Content);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);
        }
    }

    private static async Task<Ok> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var post = await database.Posts.SingleAsync(x => x.Id == request.Id, cancellationToken);
        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAtUtc = DateTime.UtcNow;
        await database.SaveChangesAsync(cancellationToken);

        // TODO: Publish post updated event
        
        return TypedResults.Ok();
    }
}