namespace Bookify.Application.Exceptions
{
    internal sealed record ValidationError(string PropertyName, string Error);
}
