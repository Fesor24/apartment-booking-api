using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories
{
    internal sealed class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
    {
        
    }
}
