using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Interfaces;
using LuseGestion.Domain.Primitives;

namespace LuseGestion.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Result<Usuario>> CreateUsuarioAsync(string email, string nombre, string password, string? telefono = null, int? idPerfil = null)
    {
        try
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(email))
                return Result<Usuario>.Failure("El email es requerido");

            if (string.IsNullOrWhiteSpace(nombre))
                return Result<Usuario>.Failure("El nombre es requerido");

            if (string.IsNullOrWhiteSpace(password))
                return Result<Usuario>.Failure("La contraseña es requerida");

            // Validar formato de email
            if (!IsValidEmail(email))
                return Result<Usuario>.Failure("Formato de email inválido");

            // Verificar si el email ya existe
            var emailExists = await _usuarioRepository.EmailExistsAsync(email);
            if (emailExists)
                return Result<Usuario>.Failure("El email ya existe");

            // Crear usuario
            var usuario = Usuario.Create(email, nombre, password, telefono, idPerfil);

            // Guardar en base de datos
            var createResult = await _usuarioRepository.CreateAsync(usuario);
            if (createResult.IsFailure)
                return Result<Usuario>.Failure(createResult.Error);

            return Result<Usuario>.Success(usuario);
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error al crear usuario: {ex.Message}");
        }
    }

    public async Task<Result<Usuario>> GetUsuarioByIdAsync(int id)
    {
        if (id <= 0)
            return Result<Usuario>.Failure("ID de usuario inválido");

        return await _usuarioRepository.GetByIdAsync(id);
    }

    public async Task<Result<Usuario>> GetUsuarioByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Usuario>.Failure("El email es requerido");

        return await _usuarioRepository.GetByEmailAsync(email);
    }

    public async Task<Result<IEnumerable<Usuario>>> GetAllUsuariosAsync()
    {
        return await _usuarioRepository.GetAllAsync();
    }

    public async Task<Result<Usuario>> UpdateUsuarioAsync(int id, string nombre, string? telefono = null, int? idPerfil = null)
    {
        try
        {
            // Validaciones
            if (id <= 0)
                return Result<Usuario>.Failure("ID de usuario inválido");

            if (string.IsNullOrWhiteSpace(nombre))
                return Result<Usuario>.Failure("El nombre es requerido");

            // Obtener usuario existente
            var getUserResult = await _usuarioRepository.GetByIdAsync(id);
            if (getUserResult.IsFailure)
                return Result<Usuario>.Failure(getUserResult.Error);

            var usuario = getUserResult.Value;

            // Actualizar usuario
            usuario.Update(nombre, telefono, idPerfil);

            // Guardar cambios
            var updateResult = await _usuarioRepository.UpdateAsync(usuario);
            if (updateResult.IsFailure)
                return Result<Usuario>.Failure(updateResult.Error);

            return Result<Usuario>.Success(usuario);
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error al actualizar usuario: {ex.Message}");
        }
    }

    public async Task<Result> DeleteUsuarioAsync(int id)
    {
        if (id <= 0)
            return Result.Failure("ID de usuario inválido");

        // Verificar que el usuario existe
        var getUserResult = await _usuarioRepository.GetByIdAsync(id);
        if (getUserResult.IsFailure)
            return Result.Failure(getUserResult.Error);

        return await _usuarioRepository.DeleteAsync(id);
    }

    public async Task<Result<Usuario>> AuthenticateAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<Usuario>.Failure("El email es requerido");

            if (string.IsNullOrWhiteSpace(password))
                return Result<Usuario>.Failure("La contraseña es requerida");

            var result = await _usuarioRepository.GetByEmailAndPasswordAsync(email, password);

            if (result.IsSuccess)
            {
                var usuario = result.Value;
                usuario.ResetearIntentosFallidos();
                await _usuarioRepository.UpdateAsync(usuario);
            }

            return result;
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error de autenticación: {ex.Message}");
        }
    }

    public async Task<Result> ChangePasswordAsync(int id, string currentPassword, string newPassword)
    {
        try
        {
            if (id <= 0)
                return Result.Failure("ID de usuario inválido");

            if (string.IsNullOrWhiteSpace(currentPassword))
                return Result.Failure("La contraseña actual es requerida");

            if (string.IsNullOrWhiteSpace(newPassword))
                return Result.Failure("La nueva contraseña es requerida");

            // Obtener usuario
            var getUserResult = await _usuarioRepository.GetByIdAsync(id);
            if (getUserResult.IsFailure)
                return Result.Failure(getUserResult.Error);

            var usuario = getUserResult.Value;

            // Verificar contraseña actual
            if (usuario.Pass != currentPassword)
                return Result.Failure("La contraseña actual es incorrecta");

            // Actualizar contraseña
            usuario.UpdatePassword(newPassword);

            return await _usuarioRepository.UpdateAsync(usuario);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al cambiar contraseña: {ex.Message}");
        }
    }

    public async Task<Result> ActivateUsuarioAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result.Failure("ID de usuario inválido");

            var getUserResult = await _usuarioRepository.GetByIdAsync(id);
            if (getUserResult.IsFailure)
                return Result.Failure(getUserResult.Error);

            var usuario = getUserResult.Value;
            usuario.Activar();

            return await _usuarioRepository.UpdateAsync(usuario);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al activar usuario: {ex.Message}");
        }
    }

    public async Task<Result> DeactivateUsuarioAsync(int id)
    {
        try
        {
            if (id <= 0)
                return Result.Failure("ID de usuario inválido");

            var getUserResult = await _usuarioRepository.GetByIdAsync(id);
            if (getUserResult.IsFailure)
                return Result.Failure(getUserResult.Error);

            var usuario = getUserResult.Value;
            usuario.Desactivar();

            return await _usuarioRepository.UpdateAsync(usuario);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error al desactivar usuario: {ex.Message}");
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
