namespace Chirper.Posts.Endpoints;

public class GetPostById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", Handle)
        .WithSummary("Gets a post by id")
        .WithRequestValidation<Request>();

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
    public record Response(
        int Id,
        string Title,
        string? Content,
        int UserId,
        string Username,
        string UserDisplayName,
        DateTime CreateAtUtc,
        DateTime? UpdatedAtUtc,
        int LikesCount,
        int CommentsCount
    );

    private static async Task<Results<Ok<Response>, NotFound>> Handle([AsParameters] Request request, AppDbContext database, CancellationToken cancellationToken)
    {
        var post = await database.Posts
            .Where(x => x.Id == request.Id)
            .Select(x => new Response
            (
                x.Id,
                x.Title,
                x.Content,
                x.UserId,
                x.User.Username,
                x.User.DisplayName,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                x.Likes.Count,
                x.Comments.Count
            ))
            .SingleOrDefaultAsync(cancellationToken);

        return post is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(post);
    }
}