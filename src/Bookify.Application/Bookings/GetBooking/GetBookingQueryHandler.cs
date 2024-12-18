﻿using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Application.Bookings.GetBooking;
internal sealed class GetBookingQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetBookingQuery, BookingResponse>
{
    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        using var connection = sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
            id AS Id
            apartment_id AS ApartmentId,
            user_id AS UserId

            FROM bookings
            WHERE id = @BookingId

            """;

        var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(
            sql, 
            new { 
                request.BookingId 
            });

        return booking;
    }
}