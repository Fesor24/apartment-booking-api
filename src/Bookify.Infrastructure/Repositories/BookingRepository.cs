using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories
{
    internal sealed class BookingRepository(ApplicationDbContext context) : Repository<Booking>(context), IBookingRepository
    {
        private static readonly BookingStatus[] ActiveBookingStatuses = 
            [
                BookingStatus.Reserved,
                BookingStatus.Confirmed,
                BookingStatus.Completed
            ];

        public Task<bool> IsOverlappingAsync(Apartment apartment, DateRange duration, CancellationToken cancellationToken)
        {
            return context.Set<Booking>()
                .AnyAsync(booking =>
                    booking.ApartmentId == apartment.Id &&
                    booking.Duration.Start <= duration.End &&
                    booking.Duration.End >= duration.Start &&
                    ActiveBookingStatuses.Contains(booking.Status),
                    cancellationToken
                );
        }
    }
}
