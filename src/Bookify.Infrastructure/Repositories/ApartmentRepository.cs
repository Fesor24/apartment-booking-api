using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Domain.Apartments;

namespace Bookify.Infrastructure.Repositories
{
    internal sealed class ApartmentRepository(ApplicationDbContext context) : Repository<Apartment>(context), IApartmentRepository
    {
    }
}
