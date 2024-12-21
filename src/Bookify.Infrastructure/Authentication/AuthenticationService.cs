using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace Bookify.Infrastructure.Authentication
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private const string PasswordCredentialType = "password";
        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            var userRepresentationModel = UserRepresentationModel.FromUser(user);

            userRepresentationModel.Credentials = new CredentialRepresentationModel[]
            {
                new()
                {
                    Value = password,
                    Temporary = false,
                    Type = PasswordCredentialType
                }
            };

            var response = await _httpClient.PostAsJsonAsync("users", userRepresentationModel, cancellationToken);

            return ExtractIdentityIdFromLocationHeader(response);
        }

        private string ExtractIdentityIdFromLocationHeader(HttpResponseMessage response)
        {
            const string usersSegmentName = "users/";

            var locationHeader = response.Headers.Location?.PathAndQuery;

            if (locationHeader is null)
                throw new InvalidOperationException("Location header can't be null");

            var userSegmentValueIndex = locationHeader.IndexOf(usersSegmentName, StringComparison.InvariantCultureIgnoreCase);

            var userIdentityId = locationHeader.Substring(userSegmentValueIndex + usersSegmentName.Length);

            return userIdentityId;
        }
    }
}
