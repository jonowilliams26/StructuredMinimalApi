namespace Chirper.Posts.Endpoints.Filters;

public interface IRequiresUserOwnsPost
{
    int Id { get; }
}

public class UserOwnsPostFilter<TRequest>(AppDbContext db) : IEndpointFilter where TRequest : IRequiresUserOwnsPost
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var ct = context.HttpContext.RequestAborted;
        var userId = context.HttpContext.User.GetUserId();

        var post = await db.Posts
            .Where(x => x.Id == request.Id)
            .Select(x => new Post(x.Id, x.Author.Id))
            .SingleOrDefaultAsync(ct);

        if (post is null)
        {
            return TypedResults.NotFound();
        }

        if (post.AuthorId != userId)
        {
            return TypedResults.Forbid();
        }

        return await next(context);
    }

    private record Post(int Id, int AuthorId);
}

public static class UserOwnsPostFilterExtensions
{
    public static RouteHandlerBuilder RequiresUserOwnsPost<TRequest>(this RouteHandlerBuilder builder) where TRequest : IRequiresUserOwnsPost
    {
        return builder.AddEndpointFilter<UserOwnsPostFilter<TRequest>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }
}