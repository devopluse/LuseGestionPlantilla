using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Primitives;

namespace LuseGestion.Domain.Interfaces;

public interface IUsuarioService
{
    Task<Result<Usuario>> CreateUsuarioAsync(string email, string nombre, string password, string? telefono = null, int? idPerfil = null);
    Task<Result<Usuario>> GetUsuarioByIdAsync(int id);
    Task<Result<Usuario>> GetUsuarioByEmailAsync(string email);
    Task<Result<IEnumerable<Usuario>>> GetAllUsuariosAsync();
    Task<Result<Usuario>> UpdateUsuarioAsync(int id, string nombre, string? telefono = null, int? idPerfil = null);
    Task<Result> DeleteUsuarioAsync(int id);
    Task<Result<Usuario>> AuthenticateAsync(string email, string password);
    Task<Result> ChangePasswordAsync(int id, string currentPassword, string newPassword);
    Task<Result> ActivateUsuarioAsync(int id);
    Task<Result> DeactivateUsuarioAsync(int id);
}
