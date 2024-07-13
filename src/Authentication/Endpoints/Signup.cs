﻿using Chirper.Authentication.Services;

namespace Chirper.Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/signup", Handle)
        .WithSummary("Creates a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Name);
    public record Response(string Token);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, ValidationError>> Handle(Request request, AppDbContext db, Jwt jwt, CancellationToken ct)
    {
        var isUsernameTaken = await db.Users
            .AnyAsync(x => x.Username == request.Username, ct);

        if (isUsernameTaken)
        {
            return new ValidationError("Username is already taken");
        }

        var user = new User
        {
            Username = request.Username,
            Password = request.Password,
            DisplayName = request.Name
        };
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);

        var token = jwt.GenerateToken(user);
        var response = new Response(token);
        return TypedResults.Ok(response);
    }
}