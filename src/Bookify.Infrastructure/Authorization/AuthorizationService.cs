using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
internal sealed class AuthorizationService(ApplicationDbContext context)
{
    public async Task<UserRoleResponse> GetRolesForUserAsync(string identityId)
    {
        var roles = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .Select(user => new UserRoleResponse
            {
                Id = user.Id,
                Roles = user.Roles.ToList()
            })
            .FirstAsync();

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var permissions = await context.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstAsync();

        var permissionSet = permissions.Select(permission => permission.Name).ToHashSet();

        return permissionSet;
    }
}
