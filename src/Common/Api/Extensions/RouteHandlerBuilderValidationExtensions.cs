using Chirper.Common.Api.Filters;

namespace Chirper.Common.Api.Extensions;

public static class RouteHandlerBuilderValidationExtensions
{
    /// <summary>
    /// Adds a request validation filter to the route handler.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="builder"></param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to futher customize the endpoint.</returns>
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Adds a request validation filter to the route handler to ensure a <typeparamref name="TEntity"/> exists with the id returned by <paramref name="idSelector"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="builder"></param>
    /// <param name="idSelector">A function which selects the <c>Id</c> property from the <typeparamref name="TRequest"/></param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to futher customize the endpoint.</returns>
    public static RouteHandlerBuilder WithEnsureEntityExists<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int?> idSelector) where TEntity : class, IEntity
    {
        return builder
            .AddEndpointFilterFactory((endpointFilterFactoryContext, next) => async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var filter = new EnsureEntityExistsFilter<TRequest, TEntity>(db, idSelector);
                return await filter.InvokeAsync(context, next);
            })
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Adds a request validation filter to the route handler to ensure the current <seealso cref="ClaimsPrincipal"/> owns the <typeparamref name="TEntity"/> with the id returned by <paramref name="idSelector"/>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="builder"></param>
    /// <param name="idSelector">A function which selects the <c>Id</c> property from the <typeparamref name="TRequest"/></param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to futher customize the endpoint.</returns>
    public static RouteHandlerBuilder WithEnsureUserOwnsEntity<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int> idSelector) where TEntity : class, IEntity, IOwnedEntity
    {
        return builder
            .AddEndpointFilterFactory((endpointFilterFactoryContext, next) => async context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var filter = new EnsureUserOwnsEntityFilter<TRequest, TEntity>(db, idSelector);
                return await filter.InvokeAsync(context, next);
            })
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }
}