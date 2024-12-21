using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Bookify.Infrastructure.Authentication
{
    internal class JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions) : 
        IConfigureNamedOptions<JwtBearerOptions>
    {
        public void Configure(string? name, JwtBearerOptions options)
        {
            Configure(options);
        }

        public void Configure(JwtBearerOptions options)
        {
            options.Audience = authenticationOptions.Value.Audience;
            options.RequireHttpsMetadata = authenticationOptions.Value.RequireHttpsMetadata;
            options.TokenValidationParameters.ValidIssuer = authenticationOptions.Value.Issuer;
            options.MetadataAddress = authenticationOptions.Value.MetadataUrl;
        }
    }
}
