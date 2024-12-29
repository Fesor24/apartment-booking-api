using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories
{
    internal sealed class UserRepository(ApplicationDbContext context) : Repository<User>(context), IUserRepository
    {
        public override void Add(User entity)
        {
            foreach (var role in entity.Roles)
                DbContext.Attach(role); // this ensures that only new entities will be added...

            DbContext.Add(entity);
        }
    }
}
