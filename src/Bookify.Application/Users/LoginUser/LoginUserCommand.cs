using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Users.LoginUser
{
    internal record LoginUserCommand(string Email, string Password) : ICommand<AccessTokenResponse>;
}
