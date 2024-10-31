namespace Bookify.Domain.Apartments;
public record Address(
    string Country,
    string ZipCode,
    string State,
    string City,
    string Street);
