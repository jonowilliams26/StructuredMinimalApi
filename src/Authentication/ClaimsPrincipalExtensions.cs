namespace System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if(!int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
        {
            throw new InvalidOperationException("Invalid UserId");
        }

        return id;
    }
}