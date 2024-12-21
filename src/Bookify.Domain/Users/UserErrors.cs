using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users;
public static class UserErrors
{
    public static Error NotFound = new("User.NotFound", "User not found");
    public static Error InvalidCredentials = new("User.InvalidCredentials", "Invalid credentials");
}
