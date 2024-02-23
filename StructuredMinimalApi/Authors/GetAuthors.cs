namespace StructuredMinimalApi.Authors;

public record GetAuthorsResponseItem(int Id, string Name);

public class GetAuthors : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/", Handle)
        .WithSummary("Gets all authors");

    private static async Task<Ok<List<GetAuthorsResponseItem>>> Handle(AppDbContext database, CancellationToken cancellationToken)
    {
        var authors = await database.Authors
            .Select(x => new GetAuthorsResponseItem(x.Id, x.Name))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(authors);
    }
}
