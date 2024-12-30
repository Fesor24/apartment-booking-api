using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authorization;
// so our authorization policies can be registered dynamically
internal class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptions = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // we call the base to see if we get a value...
        var policy =  await base.GetPolicyAsync(policyName);

        if (policy is not null)
            return policy;

        // other wise we create a new permission policy
        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        // to cache the policy value
        _authorizationOptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}
