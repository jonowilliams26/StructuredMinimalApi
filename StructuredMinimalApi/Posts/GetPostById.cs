namespace StructuredMinimalApi.Posts;

public record GetPostByIdRequest(int Id);
public record GetPostByIdResponse
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Text { get; init; }
    public required GetPostByIdResponseAuthor Author { get; init; }
}
public record GetPostByIdResponseAuthor
{
    public required int Id { get; init; } 
    public required string Name { get; init; }
}

public class GetPostByIdRequestValidator : AbstractValidator<GetPostByIdRequest>
{
    public GetPostByIdRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetPostById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithRequestValidation<GetPostByIdRequest>()
        .WithSummary("Gets a post by id")
        .WithDescription("Gets a post by id");

    public static async Task<Results<Ok<GetPostByIdResponse>, NotFound>> Handle([AsParameters] GetPostByIdRequest request, AppDbContext database, CancellationToken cancellationToken)
    {
        var post = await database.Posts
            .Select(x => new GetPostByIdResponse
            {
                Id = x.Id,
                Title = x.Title,
                Text = x.Text,
                Author = new GetPostByIdResponseAuthor
                {
                    Id = x.Author.Id, 
                    Name = x.Author.Name
                }
            })
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }
}