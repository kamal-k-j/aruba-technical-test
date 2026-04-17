using Aruba.Identity.Api.Users.Requests;
using Aruba.Identity.Api.Users.Responses;
using Aruba.Identity.Application.Users.Commands.Delete;
using Aruba.Identity.Application.Users.Commands.Update;
using Aruba.Identity.Application.Users.Queries.GetAll;
using Aruba.Identity.Application.Users.Queries.GetByEmail;
using Aruba.Identity.Application.Users.Queries.GetById;
using Aruba.Identity.Application.Users.Queries.Search;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aruba.Identity.Api.Users;

[Authorize]
[ApiController]
[Route("users")]
[Produces("application/json")]
[Consumes("application/json")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetById(string id)
    {
        var userResult = await _mediator.Send(new GetUserByIdQuery { Id = id });

        return Ok(_mapper.Map<UserResponse>(userResult));
    }

    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetByEmail([FromQuery] string email)
    {
        var userResult = await _mediator.Send(new GetUserByEmailQuery { Email = email });

        return Ok(_mapper.Map<UserResponse>(userResult));
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var usersResult = await _mediator.Send(new GetAllUsersQuery());

        return Ok(_mapper.Map<List<UserResponse>>(usersResult));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Update(string id, [FromBody] UpdateUserRequest payload)
    {
        var command = _mapper.Map<UpdateUserCommand>(payload) with { Id = id };

        var userResult = await _mediator.Send(command);

        return Ok(_mapper.Map<UserResponse>(userResult));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(string id)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id });

        return NoContent();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserResponse>>> Search([FromQuery] string field, [FromQuery] string value)
    {
        var query = new SearchUsersQuery
        {
            Field = field,
            Value = value
        };

        var users = await _mediator.Send(query);

        return Ok(_mapper.Map<List<UserResponse>>(users));
    }
}