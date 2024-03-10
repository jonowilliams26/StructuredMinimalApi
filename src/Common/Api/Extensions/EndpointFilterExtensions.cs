using Chirper.Common.Api.Filters;

namespace Microsoft.AspNetCore.Http;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder WithEnsureEntityExists<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int> idSelector) where TEntity : class, IEntity
    {
        return builder
            .AddEndpointFilter(new EnsureEntityExistsFilter<TRequest, TEntity>(idSelector))
            .Produces(StatusCodes.Status404NotFound);
    }

    public static RouteHandlerBuilder WithEnsureUserOwnsEntity<TEntity, TRequest>(this RouteHandlerBuilder builder, Func<TRequest, int> idSelector) where TEntity : class, IOwnedEntity
    {
        return builder
            .AddEndpointFilter(new EnsureUserOwnsEntityFilter<TRequest, TEntity>(idSelector))
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);
    }

    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}