namespace StructuredMinimalApi.Authors;

public record GetAuthorByIdWithPostsRequest(int Id);
public record GetAuthorByIdWithPostsResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public List<GetAuthorByIdWithPostsResponsePost> Posts { get; init; } = [];
}
public record GetAuthorByIdWithPostsResponsePost
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Text { get; init; }
}

public class GetAuthorByIdWithPostsRequestValidator : AbstractValidator<GetAuthorByIdWithPostsRequest>
{
    public GetAuthorByIdWithPostsRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class GetAuthorByIdWithPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithRequestValidation<GetAuthorByIdWithPostsRequest>()
        .WithSummary("Gets an author with posts by id")
        .WithDescription("Gets an author with posts by id");

    private static async Task<Results<Ok<GetAuthorByIdWithPostsResponse>, NotFound>> Handle([AsParameters] GetAuthorByIdWithPostsRequest request, AppDbContext database, CancellationToken cancellationToken)
    {
        var author = await database.Authors
            .Select(x => new GetAuthorByIdWithPostsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Posts = x.Posts.Select(p => new GetAuthorByIdWithPostsResponsePost
                {
                    Id = p.Id, 
                    Title = p.Title, 
                    Text = p.Text
                }).ToList()
            })
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return author is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(author);
    }
}
