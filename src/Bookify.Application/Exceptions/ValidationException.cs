namespace Bookify.Application.Exceptions
{
    internal class ValidationException(IEnumerable<ValidationError> errors) : Exception
    {
        public IEnumerable<ValidationError> Errors => errors;
    }
}
