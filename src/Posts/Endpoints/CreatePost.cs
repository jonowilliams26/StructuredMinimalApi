namespace Chirper.Posts.Endpoints;

public class CreatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithSummary("Creates a new post")
        .WithRequestValidation<Request>();

    public record Request(string Title, string? Content);
    public record Response(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);
        }
    }

    private static async Task<Ok<Response>> Handle(Request request, AppDbContext database, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            UserId = claimsPrincipal.GetUserId()
        };

        await database.Posts.AddAsync(post, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new Response(post.Id);
        return TypedResults.Ok(response);
    }
}