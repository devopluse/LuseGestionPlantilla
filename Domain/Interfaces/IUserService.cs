using MyBackendApi.Domain.Entities;
using MyBackendApi.Domain.Primitives;

namespace MyBackendApi.Domain.Interfaces;

public interface IUserService
{
    Task<Result<User>> CreateUserAsync(string email, string firstName, string lastName);
    Task<Result<User>> GetUserByIdAsync(Guid id);
    Task<Result<User>> GetUserByEmailAsync(string email);
    Task<Result<IEnumerable<User>>> GetAllUsersAsync();
    Task<Result<User>> UpdateUserAsync(Guid id, string firstName, string lastName);
    Task<Result> DeleteUserAsync(Guid id);
}
