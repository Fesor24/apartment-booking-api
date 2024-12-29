using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;
internal sealed class CustomClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
{
    // gives us access to the claims pricipal and allows us add more claims to it...
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if(principal.HasClaim(claim => claim.Type == ClaimTypes.Role) &&
            principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub)) 
        {
            // sub added bcos asp net core will transform the incoming sub claim on the jwt token into the name identifier claim
            // so the sub claim itself will not exist anymore
            return principal;
        }

        using var scope = serviceProvider.CreateScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        var identityId = principal.GetIdentityId();

        var roles = await authorizationService.GetRolesForUserAsync(identityId);

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, roles.Id.ToString()));

        foreach (var role in roles.Roles)
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));

        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}
