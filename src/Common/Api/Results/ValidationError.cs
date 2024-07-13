using Microsoft.AspNetCore.Http.Metadata;
using System.Reflection;

namespace Chirper.Common.Api.Results;

public sealed class ValidationError : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IContentTypeHttpResult, IValueHttpResult, IValueHttpResult<HttpValidationProblemDetails>
{
    private readonly ValidationProblem problem;

    public ValidationError(string errorMessage)
    {
        problem = TypedResults.ValidationProblem
        (
            errors: new Dictionary<string, string[]>(),
            detail: errorMessage
        );
    }

    public int? StatusCode => problem.StatusCode;
    public string? ContentType => problem.ContentType;
    public object? Value => problem.ProblemDetails;
    HttpValidationProblemDetails? IValueHttpResult<HttpValidationProblemDetails>.Value => problem.ProblemDetails;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status400BadRequest, typeof(HttpValidationProblemDetails), ["application/problem+json"]));
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await problem.ExecuteAsync(httpContext);
    }
}
