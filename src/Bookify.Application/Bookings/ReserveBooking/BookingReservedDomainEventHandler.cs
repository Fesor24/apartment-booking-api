using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;
internal sealed class BookingReservedDomainEventHandler(
    IUserRepository userRepository,
    IBookingRepository bookingRepository,
    IEmailService emailService
    ) : 
    INotificationHandler<BookingReservedDomainEvent>
{
    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(notification.BookingId);

        if (booking is null) return;

        var user = await userRepository.GetByIdAsync(booking.UserId);

        if (user is null) return;

        await emailService.SendAsync(
            user.Email,
            "Booking Reserved!",
            "You have 10 minutes to confirm the booking"
            );
    }
}
