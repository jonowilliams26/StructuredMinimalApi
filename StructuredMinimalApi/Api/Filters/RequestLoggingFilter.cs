namespace StructuredMinimalApi.Api.Filters;

public class RequestLoggingFilter(ILogger<RequestLoggingFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation("HTTP {Method} {Path} recieved", context.HttpContext.Request.Method, context.HttpContext.Request.Path);
        return await next(context);
    }
}