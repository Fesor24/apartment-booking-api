using Bookify.Domain.Apartments;

namespace Bookify.Domain.Bookings;
public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid Id);
    Task<bool> IsOverlappingAsync(Apartment apartment, DateRange duration, CancellationToken cancellationToken);
    Task Add(Booking booking);
}
