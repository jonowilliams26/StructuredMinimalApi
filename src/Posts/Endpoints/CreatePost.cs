namespace Chirper.Posts.Endpoints;

public class CreatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new post")
        .WithRequestValidation<Request>();

    public record Request(string Content);
    public record Response(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(250);
        }
    }

    private static async Task<Results<Ok<Response>, ValidationError>> Handle(Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var post = new Post
        {
            Content = request.Content,
            UserId = claimsPrincipal.GetUserId()
        };

        await db.Posts.AddAsync(post, ct);
        await db.SaveChangesAsync(ct);
        var response = new Response(post.Id);
        return TypedResults.Ok(response);
    }
}