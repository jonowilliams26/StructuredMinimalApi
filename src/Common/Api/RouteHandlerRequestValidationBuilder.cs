using Chirper.Common.Api.Filters;

namespace Microsoft.AspNetCore.Http;

public class RouteHandlerRequestValidationBuilder<TRequest>(RouteHandlerBuilder builder)
{
    /// <summary>
    /// Adds a filter to ensure that the specified <c>TEntity</c> exists in the database with the ID provided by the <c>idSelector</c>.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="idSelector"></param>
    /// <returns>A <see cref="RouteHandlerRequestValidationBuilder{TRequest}"/> to build additional request validation</returns>
    public RouteHandlerRequestValidationBuilder<TRequest> EnsureEntityExists<TEntity>(Func<TRequest, int?> idSelector) where TEntity : class, IEntity
    {
        builder
            .AddEndpointFilter(new EnsureEntityExistsFilter<TRequest, TEntity>(idSelector))
            .ProducesProblem(StatusCodes.Status404NotFound);

        return this;
    }

    /// <summary>
    /// Adds a filter to ensure that the specified <c>TEntity</c> exists in the database with the ID provided by the <c>idSelector</c> and that the current <see cref="ClaimsPrincipal"/> owns it.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="idSelector"></param>
    /// <returns>A <see cref="RouteHandlerRequestValidationBuilder{TRequest}"/> to build additional request validation</returns>
    public RouteHandlerRequestValidationBuilder<TRequest> EnsureUserOwnsEntity<TEntity>(Func<TRequest, int> idSelector) where TEntity : class, IOwnedEntity
    {
        builder
            .AddEndpointFilter(new EnsureUserOwnsEntityFilter<TRequest, TEntity>(idSelector))
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);

        return this;
    }
}

public static class EndpointFilterExtensions
{
    /// <summary>
    /// Adds a request validation filter to the route handler.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="builder"></param>
    /// <returns>A <see cref="RouteHandlerRequestValidationBuilder{TRequest}"/> to build additional request validation</returns>
    public static RouteHandlerRequestValidationBuilder<TRequest> WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();

        return new RouteHandlerRequestValidationBuilder<TRequest>(builder);
    }
}