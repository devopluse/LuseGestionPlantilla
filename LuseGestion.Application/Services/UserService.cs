using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Interfaces;
using LuseGestion.Domain.Primitives;

namespace LuseGestion.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> CreateUserAsync(string email, string firstName, string lastName)
    {
        try
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(email))
                return Result<User>.Failure("Email is required");

            if (string.IsNullOrWhiteSpace(firstName))
                return Result<User>.Failure("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result<User>.Failure("Last name is required");

            // Validar formato de email
            if (!IsValidEmail(email))
                return Result<User>.Failure("Invalid email format");

            // Verificar si el email ya existe
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
                return Result<User>.Failure("Email already exists");

            // Crear usuario
            var user = User.Create(email, firstName, lastName);
            
            // Guardar en base de datos
            var createResult = await _userRepository.CreateAsync(user);
            if (createResult.IsFailure)
                return Result<User>.Failure(createResult.Error);

            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Error creating user: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetUserByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return Result<User>.Failure("Invalid user ID");

        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<Result<User>> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure("Email is required");

        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<Result<IEnumerable<User>>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<Result<User>> UpdateUserAsync(Guid id, string firstName, string lastName)
    {
        try
        {
            // Validaciones
            if (id == Guid.Empty)
                return Result<User>.Failure("Invalid user ID");

            if (string.IsNullOrWhiteSpace(firstName))
                return Result<User>.Failure("First name is required");

            if (string.IsNullOrWhiteSpace(lastName))
                return Result<User>.Failure("Last name is required");

            // Obtener usuario existente
            var getUserResult = await _userRepository.GetByIdAsync(id);
            if (getUserResult.IsFailure)
                return Result<User>.Failure(getUserResult.Error);

            var user = getUserResult.Value;

            // Actualizar usuario
            user.Update(firstName, lastName);

            // Guardar cambios
            var updateResult = await _userRepository.UpdateAsync(user);
            if (updateResult.IsFailure)
                return Result<User>.Failure(updateResult.Error);

            return Result<User>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Error updating user: {ex.Message}");
        }
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        if (id == Guid.Empty)
            return Result.Failure("Invalid user ID");

        // Verificar que el usuario existe
        var getUserResult = await _userRepository.GetByIdAsync(id);
        if (getUserResult.IsFailure)
            return Result.Failure(getUserResult.Error);

        return await _userRepository.DeleteAsync(id);
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
