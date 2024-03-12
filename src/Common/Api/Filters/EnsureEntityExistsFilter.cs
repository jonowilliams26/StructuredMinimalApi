namespace Chirper.Common.Api.Filters;

public class EnsureEntityExistsFilter<TRequest, TEntity>(Func<TRequest, int?> idSelector) : IEndpointFilter
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

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var exists = await db
            .Set<TEntity>()
            .AnyAsync(x => x.Id == id, ct);

        if (!exists)
        {
            return TypedResults.Problem
            (
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: $"{typeof(TEntity).Name} with id {id} was not found."
            );
        }

        return await next(context);
    }
}