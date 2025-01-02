using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;
public static class ApartmentErrors
{
    public static Error NotFound = new("Apartment.NotFound", "Apartment not found");
}
