using Chirper.Authentication.Endpoints;
using Chirper.Comments.Endpoints;
using Chirper.Common.Api.Filters;
using Chirper.Posts.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Chirper;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        var securityScheme = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Name = JwtBearerDefaults.AuthenticationScheme,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            }
        };

        var endpoints = app.MapGroup("")
            .AddEndpointFilter<RequestLoggingFilter>()
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });

        endpoints.MapGroup("/auth")
             .WithTags("Authentication")
             .AllowAnonymous()
             .MapEndpoint<Signup>()
             .MapEndpoint<Login>();

        endpoints.MapGroup("/posts")
            .WithTags("Posts")
            .MapEndpoint<GetPosts>()
            .MapEndpoint<GetPostById>()
            .MapEndpoint<CreatePost>()
            .MapEndpoint<UpdatePost>()
            .MapEndpoint<DeletePost>()
            .MapEndpoint<LikePost>()
            .MapEndpoint<UnlikePost>();

        endpoints.MapGroup("/comments")
            .WithTags("Comments")
            .MapEndpoint<CreateComment>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}