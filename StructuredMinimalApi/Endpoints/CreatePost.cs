namespace StructuredMinimalApi.Endpoints;

public record CreatePostRequest(string Title, string Text);
public record CreatePostResponse(int Id);

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
	public CreatePostRequestValidator()
	{
		RuleFor(x => x.Title).NotEmpty();
		RuleFor(x => x.Text).NotEmpty();
	}
}

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