using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;

namespace Bookify.Infrastructure.Authentication
{
    internal sealed class JwtService : IJwtService
    {
        public async Task<Result<string>> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken)
        {
            // send to keycloak to get token...

            await Task.CompletedTask;

            return "token";
        }
    }
}
