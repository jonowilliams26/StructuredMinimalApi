using FluentValidation;
namespace Chirper.Common.Api;

public class RequestLoggingFilter(ILogger<RequestLoggingFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation("HTTP {Method} {Path} recieved", context.HttpContext.Request.Method, context.HttpContext.Request.Path);
        return await next(context);
    }
}

public class RequestValidationFilter<TRequest>(ILogger<RequestValidationFilter<TRequest>> logger, IValidator<TRequest>? validator = null) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var requestName = typeof(TRequest).FullName;

        if (validator is null)
        {
            logger.LogInformation("{Request}: No validator configured.", requestName);
            return await next(context);
        }

        logger.LogInformation("{Request}: Validating...", requestName);
        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("{Request}: Validation failed.", requestName);
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        logger.LogInformation("{Request}: Validation succeeded.", requestName);
        return await next(context);
    }
}

public static class RouteBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder) => builder
        .AddEndpointFilter<RequestValidationFilter<TRequest>>()
        .ProducesValidationProblem();
}