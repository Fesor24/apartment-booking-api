using Asp.Versioning;
using Bookify.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users
{
    [ApiController]
    [ApiVersion(ApiVersions.V1, Deprecated = true)] // specify versions that are supported...deprecated will make version 1 deprecated and v2 the only active version
    [ApiVersion(ApiVersions.V2)]
    [Route("api/v{version:apiVersion}/user")]
    public class UsersController(ISender sender) : ControllerBase
    {
        [AllowAnonymous]
        [MapToApiVersion(ApiVersions.V1)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterV1(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password
                );

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [AllowAnonymous]
        [MapToApiVersion(ApiVersions.V2)]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterV2(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password
                );

            var result = await sender.Send(command, cancellationToken);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
