namespace Chirper.Common.Api.Filters;

public class EnsureEntityExistsFilter<TRequest, TEntity>(AppDbContext db, Func<TRequest, int?> idSelector) : IEndpointFilter
    where TEntity : class, IEntity
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var ct = context.HttpContext.RequestAborted;
        var id = idSelector(request);

        if (!id.HasValue)
        {
            return await next(context);
        }

        var exists = await db
            .Set<TEntity>()
            .AnyAsync(x => x.Id == id, ct);

        return exists
            ? await next(context)
            : new NotFoundProblem($"{typeof(TEntity).Name} with id {id} was not found.");
    }
}