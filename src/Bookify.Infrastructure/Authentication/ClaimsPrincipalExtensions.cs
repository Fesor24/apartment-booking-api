using System.Security.Claims;

namespace Bookify.Infrastructure.Authentication;
internal static class ClaimsPrincipalExtensions
{
    public static string GetIdentityId(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
        throw new ApplicationException("User identity is unavailable");
}
