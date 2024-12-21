namespace Bookify.Infrastructure.Authentication.Models
{
    public sealed class CredentialRepresentationModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Temporary { get; set; }
    }
}
