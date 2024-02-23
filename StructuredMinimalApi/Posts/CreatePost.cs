namespace StructuredMinimalApi.Posts;

public record CreatePostRequest(int AuthorId, string Title, string Text);
public record CreatePostResponse(int Id);

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.AuthorId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Text).NotEmpty();
    }
}

public class CreatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithRequestValidation<CreatePostRequest>()
        .WithSummary("Creates a new post")
        .WithDescription("Creates a new post");

    public static async Task<Results<Ok<CreatePostResponse>, NotFound<ValidationError>>> Handle(CreatePostRequest request, AppDbContext database, CancellationToken cancellationToken)
    {
        var author = await database.Authors.FindAsync([request.AuthorId], cancellationToken);

        if (author is null)
        {
            return TypedResults.Extensions.NotFound("Author not found");
        }

        var post = new Post
        {
            Title = request.Title,
            Text = request.Text,
            Author = author
        };

        await database.Posts.AddAsync(post, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new CreatePostResponse(post.Id);
        return TypedResults.Ok(response);
    }
}