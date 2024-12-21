using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication.Models
{
    public sealed class AdminAuthorizationDelegatingHandler(IOptions<KeyCloakOptions> keyCloakOptions) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationToken = await GetAuthorizationToken(cancellationToken);

            request.Headers.Authorization = new System.Net.Http.Headers
                .AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authorizationToken.AccessToken);

            var httpResponseMessage = await base.SendAsync(request, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            return httpResponseMessage;
        }

        private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
        {
            var authorizationRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", keyCloakOptions.Value.AdminClientId),
                new("client_secret", keyCloakOptions.Value.AdminClientSecret),
                new("scope", "openid email"),
                new("grant_type", "client_credentials")
            };

            var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

            var authorizationRequest = new HttpRequestMessage(HttpMethod.Post,
                new Uri(keyCloakOptions.Value.TokenUrl))
            {
                Content = authorizationRequestContent
            };

            var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

            authorizationResponse.EnsureSuccessStatusCode();

            return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>() ?? 
                throw new ApplicationException();
        }
    }
}
