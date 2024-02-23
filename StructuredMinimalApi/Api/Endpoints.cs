using StructuredMinimalApi.Api.Filters;
using StructuredMinimalApi.Authors;
using StructuredMinimalApi.Posts;

namespace StructuredMinimalApi.Api;

public interface IEndpoint
{
	static abstract void Map(IEndpointRouteBuilder app);
}

public static class Endpoints
{
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
    }
}