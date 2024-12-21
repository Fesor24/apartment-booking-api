using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Authentication.Models
{
    public sealed class UserRepresentationModel
    {
        public Dictionary<string, List<string>> Attributes { get; set; }
        public long? CreatedTimeStamp { get; set; }
        public CredentialRepresentationModel[] Credentials { get; set; }
        public string Email { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? Enabled { get; set; }
        public string Id { get; set; }
        public string[] Groups { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        internal static UserRepresentationModel FromUser(User user) =>
            new()
            {
                FirstName = user.FirstName.Value,
                LastName = user.LastName.Value,
                Email = user.Email.Value,
                Username = user.Email.Value,
                Enabled = true,
                EmailVerified = true,
                CreatedTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

    }
}
