using Microsoft.AspNetCore.Http.HttpResults;
using StructuredMinimalApi.Endpoints.Filters;

namespace StructuredMinimalApi.Endpoints;

public interface IEndpoint
{
	void Map(IEndpointRouteBuilder app);
}

public interface IEndpoint<TRequest, TResponse> : IEndpoint
{
	Task<EndpointResult<TResponse>> Handle(TRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}

[GenerateOneOf]
public partial class EndpointResult<TResponse> : OneOfBase<TResponse, ValidationError, Unathorised> { }
public record ValidationError(string Message);
public readonly record struct Unathorised;


public static class IEndpointRouteBuilderExtensions
{
	public static RouteHandlerBuilder MapRPC<TRequest, TResponse>(this IEndpointRouteBuilder app, string? path = null)
	{
		var endpointPath = path ?? typeof(TRequest).Name.Replace("Request", string.Empty);
		return app
			.MapPost(endpointPath, Handle<TRequest, TResponse>)
			.AddEndpointFilter<RequestLoggingFilter<TRequest>>()
			.AddEndpointFilter<RequestValidationFilter<TRequest>>();
	}

	private static async Task<Results<Ok<TResponse>, BadRequest<ValidationError>, UnauthorizedHttpResult>> Handle<TRequest, TResponse>(TRequest request, IEndpoint<TRequest, TResponse> endpoint, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
	{
		var result = await endpoint.Handle(request, claimsPrincipal, cancellationToken);
		return result.Match<Results<Ok<TResponse>, BadRequest<ValidationError>, UnauthorizedHttpResult>>
		(
			response => TypedResults.Ok(response),
			validationError => TypedResults.BadRequest(validationError),
			_ => TypedResults.Unauthorized()
		);
	}
}
