using Chirper.Common.Api.Filters;

namespace Microsoft.AspNetCore.Http;

public class RouteHandlerValidationBuilder<TRequest>(RouteHandlerBuilder builder)
{
    public RouteHandlerValidationBuilder<TRequest> EnsureEntityExists<TEntity>(Func<TRequest, int?> idSelector) where TEntity : class, IEntity
    {
        builder
            .AddEndpointFilter(new EnsureEntityExistsFilter<TRequest, TEntity>(idSelector))
            .ProducesProblem(StatusCodes.Status404NotFound);

        return this;
    }

    public RouteHandlerValidationBuilder<TRequest> EnsureUserOwnsEntity<TEntity>(Func<TRequest, int> idSelector) where TEntity : class, IOwnedEntity
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
    public static RouteHandlerValidationBuilder<TRequest> WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();

        return new RouteHandlerValidationBuilder<TRequest>(builder);
    }
}