# Structured Minimal API
An opinionated starter project using Minimal APIs with RPC style endpoints following the Request-Endpoint-Response (REPR) pattern.

## Opinionated Design Decisions
1. All endpoints are a `HTTP POST` with a `JSON Request Body`
2. Follows the [REPR Pattern](https://ardalis.com/mvc-controllers-are-dinosaurs-embrace-api-endpoints/) where each endpoint is its own class with its own request/response.
3. Only the following Status Codes are allowed: `200 OK`, `400 Bad Request`, `401 Unathorized`, `500 Server Error`

## Why RPC over REST?
In my opinion and personal experience, RPC api's are easier to consume since routes are more descriptive like method names and everything required is in the body.
No longer need to deal with query params, route params etc. It's all in the body.

## Creating an Endpoint
The following code will register the endpoint `HTTP POST /CreatePost`

```csharp
// Request & Response DTOs
public record CreatePostRequest(string Title, string Text);
public record CreatePostResponse(int Id);

// Request Validation Using FluentValidation
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
  	public CreatePostRequestValidator()
  	{
    		RuleFor(x => x.Title).NotEmpty();
    		RuleFor(x => x.Text).NotEmpty();
  	}
}

// Endpoint
public class CreatePost(AppDbContext database) : IEndpoint<CreatePostRequest, CreatePostResponse>
{
  	public void Map(IEndpointRouteBuilder app) => app
  		.MapRPC<CreatePostRequest, CreatePostResponse>()
  		.WithSummary("Creates a new post")
  		.WithDescription("Creates a new post");

  	public async Task<EndpointResult<CreatePostResponse>> Handle(CreatePostRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
  	{
    		var post = new Post
    		{
    			Title = request.Title,
    			Text = request.Text
    		};
    
    		await database.Posts.AddAsync(post, cancellationToken);
    		await database.SaveChangesAsync(cancellationToken);
    		return new CreatePostResponse(post.Id);
  	}
}
```

## Dependency Injection
Services can be injected into the `Endpoint` via standard constructor injection.

## Request Validation
Out of the box request validation is included using [FluentValidation.](https://github.com/FluentValidation/FluentValidation)
Request validators are registered automatically to the DI container.
