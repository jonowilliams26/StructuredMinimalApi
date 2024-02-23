# Structured Minimal API
An opinionated starter project using Minimal APIs following the Request-Endpoint-Response (REPR) pattern.

## Opinionated Design Decisions
1. Follows the [REPR Pattern](https://ardalis.com/mvc-controllers-are-dinosaurs-embrace-api-endpoints/) where each endpoint is its own class with its own request/response.
2. Each endpoint will define their `Request`/`Response` contracts, rather than reusing common DTOs each endpoint will define its own contract. This will save alot of pain later.
3. Minimize the amount of magic / abstraction. We want to be able to easily follow whats going on in the code. We dont want any MediatR magic and unnecessary abstractions. e.g. Not creating an `IRepository` ontop of `DbContext` (controversial I know)

## 1. Create a Endpoint
```csharp
// Request / Response Contracts
public record CreateAuthorRequest(string Name);
public record CreateAuthorResponse(int Id);

// Request Validation using FluentValidation
public class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

// Endpoint which maps the route and the handler
public class CreateAuthor : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", Handle)
        .WithRequestValidation<CreateAuthorRequest>() // Add Request Validation
        .WithSummary("Creates an author")
        .WithDescription("Creates an author");

    private static async Task<Ok<CreateAuthorResponse>> Handle(CreateAuthorRequest request, AppDbContext database, CancellationToken cancellationToken)
    {
        var author = new Author 
        { 
            Name = request.Name 
        };
        await database.Authors.AddAsync(author, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);
        var response = new CreateAuthorResponse(author.Id);
        return TypedResults.Ok(response);
    }
}
```

## 2. Register your Endpoint
Register your endpoint inside the `Api/Endpoints.cs` file
```csharp
public static void MapEndpoints(this WebApplication app)
{
    var endpoints = app.MapGroup("")
        .WithOpenApi()
        .AddEndpointFilter<RequestLoggingFilter>();

    endpoints.MapGroup("/posts")
        .WithTags("Posts")
        .MapEndpoint<GetPostById>()
        .MapEndpoint<CreatePost>();

    endpoints.MapGroup("/authors")
        .WithTags("Authors")
        .MapEndpoint<GetAuthors>()
        .MapEndpoint<CreateAuthor>()
        .MapEndpoint<GetAuthorByIdWithPosts>();

    // Add more endpoints here
}
```

## Dependency Injection
Services can be injected into the `Handle` method via method injection which is supported in Minimal APIs.

## Request Validation
Out of the box request validation is included using [FluentValidation.](https://github.com/FluentValidation/FluentValidation)
Request validators are registered automatically to the DI container.
To add request validation to an endpoint call `.WithRequestValidatation<TRequest>()`
