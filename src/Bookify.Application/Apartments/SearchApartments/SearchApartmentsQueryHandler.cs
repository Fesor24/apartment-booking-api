using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Apartments.SearchApartments
{
    internal sealed class SearchApartmentsQueryHandler(ISqlConnectionFactory connectionFactory) : 
        IQueryHandler<SearchApartmentsQuery, IReadOnlyList<ApartmentResponse>>
    {
        private readonly static int[] ActiveBookingStatuses = 
            [
                (int)BookingStatus.Confirmed,
                (int)BookingStatus.Reserved,
                (int)BookingStatus.Completed
            ];

        public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentsQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate) return new List<ApartmentResponse>();

            using var connection = connectionFactory.CreateConnection();

            // By using splt on below, Id to Description would be mapped to one object
            // Country to City below would be mapped to a different object...

            const string sql = """
                SELECT 
                a.id AS Id,
                a.name AS Name,
                a.description AS Description
                a.country AS Country
                a.state AS State
                A.city AS City

                FROM apartments AS a
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM bookings AS b
                    WHERE 
                        b.apartment_id = a.id AND 
                        b.duration_start <= @EndDate AND
                        b.duration_end >= @StartDate AND
                        b.Status = ANY(@ActiveBookingStatuses)
                )

                """;

            // Map the first object to apartment response and the second to address response and the final object is apartment response
            var apartments = await connection
                .QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
                sql, 
                (apartment, address) =>
                {
                    apartment.Address = address;

                    return apartment;
                },    
                new
                {
                    request.EndDate,
                    request.StartDate,
                    ActiveBookingStatuses
                }, 
                splitOn:"Country");

            return apartments.ToList();
        }
    }
}
