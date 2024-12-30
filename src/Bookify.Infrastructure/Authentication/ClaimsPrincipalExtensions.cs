using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authentication;
internal static class ClaimsPrincipalExtensions
{
    public static string GetIdentityId(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
        throw new ApplicationException("User identity is unavailable");

    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal?.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId : 
            throw new ApplicationException("User identity is unavailable");
    }
        
}
