namespace StructuredMinimalApi.Endpoints.Filters;

public class RequestValidationFilter<TRequest>(
	ILogger<RequestValidationFilter<TRequest>> logger,
	IValidator<TRequest>? validator = null) : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var requestName = typeof(TRequest).Name;

		if (validator is null)
		{
			logger.LogInformation("{Request}: No validator configured.", requestName);
			return await next(context);
		}

		logger.LogInformation("{Request}: Validating...", requestName);
		var request = context.GetArgument<TRequest>(0);
		var cancellationToken = context.GetArgument<CancellationToken>(context.Arguments.Count - 1);
		
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			var validationError = new ValidationError(validationResult.Errors.First().ErrorMessage);
			logger.LogWarning("{Request}: Validation failed. Reason: {ValidationError}.", requestName, validationError.Message);
			return TypedResults.BadRequest(validationError);
		}

		logger.LogInformation("{Request}: Validation succeeded.", requestName);
		return await next(context);
	}
}