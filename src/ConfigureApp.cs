using Chirper.Authentication.Endpoints;
using Chirper.Posts.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Chirper;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.MapEndpoints();
        await app.EnsureDatabaseCreated();
    }

    private static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
            .WithOpenApi()
            .AddEndpointFilter<RequestLoggingFilter>();

        var publicEndpoints = endpoints.MapGroup("")
            .AllowAnonymous();

        var authorizedEndpoints = endpoints.MapGroup("")
            .RequireAuthorization()
            .WithAuthorizedOpenApi();

        publicEndpoints.MapGroup("/auth")
            .WithTags("Authentication")
            .MapEndpoint<Signup>()
            .MapEndpoint<Login>();

        authorizedEndpoints.MapGroup("/posts")
            .WithTags("Posts")
            .MapEndpoint<GetPosts>()
            .MapEndpoint<GetPostById>()
            .MapEndpoint<CreatePost>()
            .MapEndpoint<UpdatePost>()
            .MapEndpoint<DeletePost>()
            .MapEndpoint<LikePost>();
    }

    private static RouteGroupBuilder WithAuthorizedOpenApi(this RouteGroupBuilder builder)
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

        return builder.WithOpenApi(o => new(o)
        {
            Security = [new() { [securityScheme] = [] }],
        });
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }

    private static async Task EnsureDatabaseCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}