namespace StructuredMinimalApi.Endpoints;

public record GetPostByIdRequest(int Id);
public record GetPostByIdResponse(int Id, string Title, string Text);

public class GetPostByIdRequestValidator : AbstractValidator<GetPostByIdRequest>
{
	public GetPostByIdRequestValidator()
	{
		RuleFor(x => x.Id).GreaterThan(0);
	}
}

public class GetPostById(AppDbContext database) : IEndpoint<GetPostByIdRequest, GetPostByIdResponse>
{
	public void Map(IEndpointRouteBuilder app) => app
		.MapRPC<GetPostByIdRequest, GetPostByIdResponse>()
		.WithSummary("Gets a post by id")
		.WithDescription("Gets a post by id");

	public async Task<EndpointResult<GetPostByIdResponse>> Handle(GetPostByIdRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
	{
		var post = await database.Posts
			.AsNoTracking()
			.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		
		if (post is null)
		{
			return new ValidationError("Post not found");
		}

		return new GetPostByIdResponse(post.Id, post.Title, post.Text);
	}
}
