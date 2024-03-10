namespace Chirper.Common.Api.Filters;

public class EnsureEntityExistsFilter<TRequest, TEntity>(Func<TRequest, int> idSelector) : IEndpointFilter
    where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var ct = context.HttpContext.RequestAborted;
        var id = idSelector(request);

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var exists = await db
            .Set<TEntity>()
            .AnyAsync(x => x.Id == id, ct);

        if (!exists)
        {
            return TypedResults.NotFound();
        }

        return await next(context);
    }
}