using Dapper;
using MySqlConnector;
using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Interfaces;
using LuseGestion.Domain.Primitives;
using LuseGestion.Infrastructure.Context;
using System.Data;

namespace LuseGestion.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMySqlConnectionFactory _connectionFactory;

    public UserRepository(IMySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, Email, FirstName, LastName, IsActive, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Id = @Id";

            var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
            
            return user != null 
                ? Result<User>.Success(user) 
                : Result<User>.Failure("User not found");
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result<User>> GetByEmailAsync(string email)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, Email, FirstName, LastName, IsActive, CreatedAt, UpdatedAt
                FROM Users 
                WHERE Email = @Email";

            var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email.ToLowerInvariant() });
            
            return user != null 
                ? Result<User>.Success(user) 
                : Result<User>.Failure("User not found");
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Id, Email, FirstName, LastName, IsActive, CreatedAt, UpdatedAt
                FROM Users 
                WHERE IsActive = 1
                ORDER BY CreatedAt DESC";

            var users = await connection.QueryAsync<User>(sql);
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<User>>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result<Guid>> CreateAsync(User user)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Users (Id, Email, FirstName, LastName, IsActive, CreatedAt)
                VALUES (@Id, @Email, @FirstName, @LastName, @IsActive, @CreatedAt)";

            var affectedRows = await connection.ExecuteAsync(sql, user);
            
            return affectedRows > 0 
                ? Result<Guid>.Success(user.Id) 
                : Result<Guid>.Failure("Failed to create user");
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(User user)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Users 
                SET FirstName = @FirstName, LastName = @LastName, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new 
            { 
                user.FirstName, 
                user.LastName, 
                UpdatedAt = DateTime.UtcNow,
                user.Id 
            });
            
            return affectedRows > 0 
                ? Result.Success() 
                : Result.Failure("Failed to update user");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE Users SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new 
            { 
                UpdatedAt = DateTime.UtcNow,
                Id = id 
            });
            
            return affectedRows > 0 
                ? Result.Success() 
                : Result.Failure("Failed to delete user");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.ToLowerInvariant() });
            return count > 0;
        }
        catch
        {
            return false;
        }
    }
}
