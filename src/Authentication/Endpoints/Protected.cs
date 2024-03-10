
namespace Chirper.Authentication.Endpoints;

public class Protected : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/protected", () => "Protected")
        .RequireAuthorization();
}
