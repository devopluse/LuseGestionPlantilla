using Microsoft.AspNetCore.Mvc;
using LuseGestion.API.DTOs;
using LuseGestion.Application.Services;
using LuseGestion.Domain.Interfaces;
using LuseGestion.Domain.Primitives;
using Swashbuckle.AspNetCore.Annotations;

namespace LuseGestion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create",
        Description = "Crea un nuevo usuario en el sistema",
        OperationId = "CreateUsuario",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUsuario([FromBody] CreateUsuarioRequest request)
    {
        var result = await _usuarioService.CreateUsuarioAsync(
            request.Email, 
            request.Nombre, 
            request.Password,
            request.Telefono,
            request.IDPerfil);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var usuarioResponse = UsuarioResponse.FromDomainUsuario(result.Value);
        return CreatedAtAction(nameof(GetUsuarioById), new { id = usuarioResponse.Id }, usuarioResponse);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "GetById",
        Description = "Obtiene un usuario por su ID",
        OperationId = "GetUsuarioById",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsuarioById(int id)
    {
        var result = await _usuarioService.GetUsuarioByIdAsync(id);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        var usuarioResponse = UsuarioResponse.FromDomainUsuario(result.Value);
        return Ok(usuarioResponse);
    }

    [HttpGet("by-email/{email}")]
    [SwaggerOperation(
        Summary = "GetByEmail",
        Description = "Obtiene un usuario por su dirección de email",
        OperationId = "GetUsuarioByEmail",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsuarioByEmail(string email)
    {
        var result = await _usuarioService.GetUsuarioByEmailAsync(email);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        var usuarioResponse = UsuarioResponse.FromDomainUsuario(result.Value);
        return Ok(usuarioResponse);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "GetAll",
        Description = "Obtiene todos los usuarios activos del sistema",
        OperationId = "GetAllUsuarios",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllUsuarios()
    {
        var result = await _usuarioService.GetAllUsuariosAsync();

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        var usuarioResponses = result.Value.Select(UsuarioResponse.FromDomainUsuario);
        return Ok(usuarioResponses);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Update",
        Description = "Actualiza la información de un usuario existente",
        OperationId = "UpdateUsuario",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UpdateUsuarioRequest request)
    {
        var result = await _usuarioService.UpdateUsuarioAsync(
            id, 
            request.Nombre, 
            request.Telefono,
            request.IDPerfil);

        if (result.IsFailure)
        {
            if (result.Error.Contains("no encontrado"))
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        var usuarioResponse = UsuarioResponse.FromDomainUsuario(result.Value);
        return Ok(usuarioResponse);
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Delete",
        Description = "Elimina (desactiva) un usuario del sistema",
        OperationId = "DeleteUsuario",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        var result = await _usuarioService.DeleteUsuarioAsync(id);

        if (result.IsFailure)
        {
            if (result.Error.Contains("no encontrado"))
                return NotFound(new { error = result.Error });

            return BadRequest(new { error = result.Error });
        }

        return NoContent();
    }

    [HttpPost("authenticate")]
    [SwaggerOperation(
        Summary = "Authenticate",
        Description = "Autentica un usuario con email y contraseña",
        OperationId = "Authenticate",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        var result = await _usuarioService.AuthenticateAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return Unauthorized(new { error = result.Error });
        }

        var usuarioResponse = UsuarioResponse.FromDomainUsuario(result.Value);
        return Ok(usuarioResponse);
    }

    [HttpPost("{id:int}/change-password")]
    [SwaggerOperation(
        Summary = "ChangePassword",
        Description = "Cambia la contraseña de un usuario",
        OperationId = "ChangePassword",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        var result = await _usuarioService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }

    [HttpPut("{id:int}/activate")]
    [SwaggerOperation(
        Summary = "Activate",
        Description = "Activa una cuenta de usuario desactivada",
        OperationId = "ActivateUsuario",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateUsuario(int id)
    {
        var result = await _usuarioService.ActivateUsuarioAsync(id);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Usuario activado exitosamente" });
    }

    [HttpPut("{id:int}/deactivate")]
    [SwaggerOperation(
        Summary = "Deactivate",
        Description = "Desactiva una cuenta de usuario activa",
        OperationId = "DeactivateUsuario",
        Tags = new[] { "Usuarios" }
    )]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeactivateUsuario(int id)
    {
        var result = await _usuarioService.DeactivateUsuarioAsync(id);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Usuario desactivado exitosamente" });
    }
}

public class AuthenticateRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
