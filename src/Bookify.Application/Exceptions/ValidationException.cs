using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.Exceptions
{
    internal class ValidationException(IEnumerable<ValidationError> errors) : Exception
    {
        public IEnumerable<ValidationError> Errors => errors;
    }
}
