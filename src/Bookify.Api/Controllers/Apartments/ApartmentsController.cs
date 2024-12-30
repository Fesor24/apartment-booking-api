using Bookify.Application.Apartments.SearchApartments;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments
{
    [ApiController]
    [Route("api/apartments")]
    public class ApartmentsController(ISender sender) : ControllerBase
    {
        [HttpGet]
        //[Authorize(Roles = Roles.Registered)] will look for role in the jwt token
        [HasPermission(Permissions.UsersRead)]
        public async Task<IActionResult> Search(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            var query = new SearchApartmentsQuery(startDate, endDate);

            var result = await sender.Send(query, cancellationToken);

            return Ok(result.Value);
        }
    }
}
