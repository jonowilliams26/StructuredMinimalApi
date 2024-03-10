namespace Chirper.Common.Api.Filters;

public class EnsureUserOwnsEntityFilter<TRequest, TEntity>(Func<TRequest, int> idSelector) : IEndpointFilter
    where TEntity : class, IOwnedEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var cancellationToken = context.HttpContext.RequestAborted;
        var userId = context.HttpContext.User.GetUserId();
        var id = idSelector(request);

        var database = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var entity = await database
            .Set<TEntity>()
            .Where(x => x.Id == id)
            .Select(x => new Entity(x.Id, x.UserId))
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        if (entity.UserId != userId)
        {
            return TypedResults.Forbid();
        }

        return await next(context);
    }

    private record Entity(int Id, int UserId);
}