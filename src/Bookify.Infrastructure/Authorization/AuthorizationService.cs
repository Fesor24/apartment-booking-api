using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
internal sealed class AuthorizationService(ApplicationDbContext context, ICacheService cacheService)
{
    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        string key = $"auth:roles-{identityId}";

        var rolesResponse = await cacheService.GetAsync<UserRoleResponse>(key);

        if (rolesResponse is not null) return rolesResponse;

        var roles = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRoleResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        await cacheService.SetAsync(key, roles);

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        // can be cached in similar...
        var permissions = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        var permissionSet = permissions.Select(permission => permission.Name).ToHashSet();

        return permissionSet;
    }
}
