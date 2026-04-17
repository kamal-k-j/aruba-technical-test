using Aruba.Identity.Api.Auth.Requests;
using Aruba.Identity.Api.Auth.Responses;
using Aruba.Identity.Api.Users.Responses;
using Aruba.Identity.Application.Auth.Commands.Login;
using Aruba.Identity.Application.Auth.Commands.Register;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aruba.Identity.Api.Auth;

[AllowAnonymous]
[ApiController]
[Route("auth")]
[Produces("application/json")]
[Consumes("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest payload)
    {
        var command = _mapper.Map<RegisterCommand>(payload);

        var userResult = await _mediator.Send(command);

        return Ok(_mapper.Map<UserResponse>(userResult));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest payload)
    {
        var command = _mapper.Map<LoginCommand>(payload);

        var loginResponse = await _mediator.Send(command);

        return Ok(loginResponse);
    }
}