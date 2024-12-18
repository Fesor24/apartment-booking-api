﻿using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;
internal sealed class ReserveBookingCommandHandler(
    IUserRepository userRepository,
    IApartmentRepository apartmentRepository,
    IBookingRepository bookingRepository,
    IUnitOfWork unitOfWork,
    PricingService pricingService,
    IDateTimeProvider dateTimeProvider
    ) : ICommandHandler<ReserveBookingCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user is null) return Result.Failure<Guid>(UserErrors.NotFound);

        var apartment = await apartmentRepository.GetByIdAsync(request.ApartmentId);

        if (apartment is null) return Result.Failure<Guid>(ApartmentErrors.NotFound);

        var duration = DateRange.Create(request.StartDate, request.EndDate);

        if (await bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
            return Result.Failure<Guid>(BookingErrors.Overlap);

        var booking = Booking.Reserve(
            apartment,
            user.Id,
            utcNow: dateTimeProvider.UtcNow,
            duration,
            pricingService
            );

        await bookingRepository.Add(booking);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.Id;
    }
}