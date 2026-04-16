using Microsoft.AspNetCore.Mvc;
using MyBackendApi.API.DTOs;
using MyBackendApi.Application.Services;
using MyBackendApi.Domain.Interfaces;
using MyBackendApi.Domain.Primitives;

namespace MyBackendApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request.Email, request.FirstName, request.LastName);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var userResponse = UserResponse.FromDomainUser(result.Value);
        return CreatedAtAction(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        var userResponse = UserResponse.FromDomainUser(result.Value);
        return Ok(userResponse);
    }

    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await _userService.GetUserByEmailAsync(email);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        var userResponse = UserResponse.FromDomainUser(result.Value);
        return Ok(userResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var userResponses = result.Value.Select(UserResponse.FromDomainUser);
        return Ok(userResponses);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUserAsync(id, request.FirstName, request.LastName);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(new { error = result.Error });
            
            return BadRequest(new { error = result.Error });
        }

        var userResponse = UserResponse.FromDomainUser(result.Value);
        return Ok(userResponse);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
                return NotFound(new { error = result.Error });
            
            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }
}
