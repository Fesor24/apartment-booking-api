namespace Bookify.Api.Controllers.Bookings
{
    public record ReserveBookingRequest(
        DateOnly StartDate,
        DateOnly EndDate,
        Guid UserId,
        Guid ApartmentId
        );
}
