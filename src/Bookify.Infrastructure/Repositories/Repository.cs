using Bookify.Domain.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories
{
    internal abstract class Repository<T>(ApplicationDbContext context) where T: Entity
    {
        protected readonly ApplicationDbContext DbContext = context;

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public virtual void Add(T entity)
        {
            DbContext.Add(entity);
        }
    }
}
