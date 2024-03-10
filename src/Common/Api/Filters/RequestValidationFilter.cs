namespace Chirper.Common.Api.Filters;

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