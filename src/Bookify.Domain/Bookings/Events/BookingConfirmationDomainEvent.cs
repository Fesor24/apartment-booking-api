using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings.Events;
public record BookingConfirmationDomainEvent(Guid BookingId) : IDomainEvent;

