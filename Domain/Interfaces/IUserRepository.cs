using MyBackendApi.Domain.Entities;
using MyBackendApi.Domain.Primitives;

namespace MyBackendApi.Domain.Interfaces;

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
