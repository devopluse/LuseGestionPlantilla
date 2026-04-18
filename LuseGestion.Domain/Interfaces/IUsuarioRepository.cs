using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Primitives;

namespace LuseGestion.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Result<Usuario>> GetByIdAsync(int id);
    Task<Result<Usuario>> GetByEmailAsync(string email);
    Task<Result<IEnumerable<Usuario>>> GetAllAsync();
    Task<Result<int>> CreateAsync(Usuario usuario);
    Task<Result> UpdateAsync(Usuario usuario);
    Task<Result> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<Result<Usuario>> GetByEmailAndPasswordAsync(string email, string password);
}
