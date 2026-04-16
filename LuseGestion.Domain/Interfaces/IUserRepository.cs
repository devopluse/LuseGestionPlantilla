using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Primitives;

namespace LuseGestion.Domain.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByEmailAsync(string email);
    Task<Result<IEnumerable<User>>> GetAllAsync();
    Task<Result<Guid>> CreateAsync(User user);
    Task<Result> UpdateAsync(User user);
    Task<Result> DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
}
