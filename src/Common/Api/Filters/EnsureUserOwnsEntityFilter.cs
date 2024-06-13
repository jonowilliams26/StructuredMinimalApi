namespace Chirper.Common.Api.Filters;

public class EnsureUserOwnsEntityFilter<TRequest, TEntity>(AppDbContext db, Func<TRequest, int> idSelector) : IEndpointFilter
    where TEntity : class, IOwnedEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var ct = context.HttpContext.RequestAborted;
        var userId = context.HttpContext.User.GetUserId();
        var id = idSelector(request);

        var entity = await db
            .Set<TEntity>()
            .Where(x => x.Id == id)
            .Select(x => new Entity(x.Id, x.UserId))
            .SingleOrDefaultAsync(ct);

        return entity switch
        {
            null => TypedResults.Problem
            (
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: $"{typeof(TEntity).Name} with id {id} was not found."
            ),
            _ when entity.UserId != userId => TypedResults.Forbid(),
            _ => await next(context)
        };
    }

    private record Entity(int Id, int UserId);
}