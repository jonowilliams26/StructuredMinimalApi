namespace StructuredMinimalApi.Authors;

public record CreateAuthorRequest(string Name);
public record CreateAuthorResponse(int Id);
public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class CreateAuthor : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", Handle)
        .WithRequestValidation<CreateAuthorRequest>()
        .WithSummary("Creates an author")
        .WithDescription("Creates an author");

    private static async Task<Ok<CreateAuthorResponse>> Handle(CreateAuthorRequest request, AppDbContext database, CancellationToken cancellationToken)
    {
        var author = new Author 
        { 
            Name = request.Name 
        };
        await database.Authors.AddAsync(author, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new CreateAuthorResponse(author.Id);
        return TypedResults.Ok(response);
    }
}
