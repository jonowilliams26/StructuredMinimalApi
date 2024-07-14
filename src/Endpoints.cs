using Chirper.Authentication.Endpoints;
using Chirper.Comments.Endpoints;
using Chirper.Common.Api.Filters;
using Chirper.Posts.Endpoints;
using Chirper.Users.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Chirper;

public static class Endpoints
{
    private static readonly OpenApiSecurityScheme securityScheme = new()
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

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
            .AddEndpointFilter<RequestLoggingFilter>()
            .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapPostEndpoints();
        endpoints.MapCommentEndpoints();
        endpoints.MapUserEndpoints();
    }

    private static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Authentication");
            
        endpoints.MapPublicGroup()
            .MapEndpoint<Signup>()
            .MapEndpoint<Login>();
    }

    private static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/posts")
            .WithTags("Posts");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetPosts>()
            .MapEndpoint<GetPostById>()
            .MapEndpoint<GetPostComments>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreatePost>()
            .MapEndpoint<UpdatePost>()
            .MapEndpoint<DeletePost>()
            .MapEndpoint<LikePost>()
            .MapEndpoint<UnlikePost>();
    }

    private static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/comments")
            .WithTags("Comments");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetCommentReplies>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<CreateComment>()
            .MapEndpoint<LikeComment>()
            .MapEndpoint<UnlikeComment>();
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/users")
            .WithTags("Users");

        endpoints.MapPublicGroup()
            .MapEndpoint<GetUserPosts>()
            .MapEndpoint<GetUserComments>()
            .MapEndpoint<GetUserFollowers>()
            .MapEndpoint<GetUserFollowing>()
            .MapEndpoint<GetUserLikedPosts>()
            .MapEndpoint<GetUserLikedComments>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<FollowUser>()
            .MapEndpoint<UnfollowUser>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}