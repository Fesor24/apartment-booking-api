using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Infrastructure.Authentication
{
    public sealed class AuthenticationOptions
    {
        public string Audience { get; init; } = "";
        public string MetadataUrl { get; init; } = "";
        public bool RequireHttpsMetadata { get; init; }
        public string Issuer { get; set; } = "";
    }
}
