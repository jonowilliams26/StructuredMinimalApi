namespace StructuredMinimalApi.Endpoints.Filters;

public class RequestLoggingFilter<TRequest>(ILogger<RequestLoggingFilter<TRequest>> logger) : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		logger.LogInformation("{Request}: Executing...", typeof(TRequest).Name);
		return await next(context);
	}
}