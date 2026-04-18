using Dapper;
using MySqlConnector;
using LuseGestion.Domain.Entities;
using LuseGestion.Domain.Interfaces;
using LuseGestion.Domain.Primitives;
using LuseGestion.Infrastructure.Context;
using System.Data;

namespace LuseGestion.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMySqlConnectionFactory _connectionFactory;

    public UsuarioRepository(IMySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<Usuario>> GetByIdAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT IDUsuario as Id, Nombre, Email, Pass, EmailConfirmado, 
                       EmailConfirmadoToken, EmailConfirmadoTokenExpira, Telefono, 
                       FechaCambioPass, IntentosFallidos, Activo, IDPerfil
                FROM usuario 
                WHERE IDUsuario = @Id";

            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(sql, new { Id = id });

            return usuario != null 
                ? Result<Usuario>.Success(usuario) 
                : Result<Usuario>.Failure("Usuario no encontrado");
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result<Usuario>> GetByEmailAsync(string email)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT IDUsuario as Id, Nombre, Email, Pass, EmailConfirmado, 
                       EmailConfirmadoToken, EmailConfirmadoTokenExpira, Telefono, 
                       FechaCambioPass, IntentosFallidos, Activo, IDPerfil
                FROM usuario 
                WHERE Email = @Email";

            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(sql, new { Email = email.ToLowerInvariant() });

            return usuario != null 
                ? Result<Usuario>.Success(usuario) 
                : Result<Usuario>.Failure("Usuario no encontrado");
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result<Usuario>> GetByEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT IDUsuario as Id, Nombre, Email, Pass, EmailConfirmado, 
                       EmailConfirmadoToken, EmailConfirmadoTokenExpira, Telefono, 
                       FechaCambioPass, IntentosFallidos, Activo, IDPerfil
                FROM usuario 
                WHERE Email = @Email AND Pass = @Password AND Activo = 1";

            var usuario = await connection.QuerySingleOrDefaultAsync<Usuario>(sql, new 
            { 
                Email = email.ToLowerInvariant(),
                Password = password 
            });

            return usuario != null 
                ? Result<Usuario>.Success(usuario) 
                : Result<Usuario>.Failure("Credenciales inválidas");
        }
        catch (Exception ex)
        {
            return Result<Usuario>.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Usuario>>> GetAllAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT IDUsuario as Id, Nombre, Email, Pass, EmailConfirmado, 
                       EmailConfirmadoToken, EmailConfirmadoTokenExpira, Telefono, 
                       FechaCambioPass, IntentosFallidos, Activo, IDPerfil
                FROM usuario 
                WHERE Activo = 1
                ORDER BY IDUsuario DESC";

            var usuarios = await connection.QueryAsync<Usuario>(sql);
            return Result<IEnumerable<Usuario>>.Success(usuarios);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Usuario>>.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(Usuario usuario)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO usuario (Nombre, Email, Pass, EmailConfirmado, Telefono, 
                                    IntentosFallidos, Activo, IDPerfil)
                VALUES (@Nombre, @Email, @Pass, @EmailConfirmado, @Telefono, 
                        @IntentosFallidos, @Activo, @IDPerfil);
                SELECT LAST_INSERT_ID();";

            var id = await connection.ExecuteScalarAsync<int>(sql, new
            {
                usuario.Nombre,
                usuario.Email,
                usuario.Pass,
                usuario.EmailConfirmado,
                usuario.Telefono,
                usuario.IntentosFallidos,
                usuario.Activo,
                usuario.IDPerfil
            });

            usuario.Id = id;

            return id > 0 
                ? Result<int>.Success(id) 
                : Result<int>.Failure("Error al crear usuario");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Usuario usuario)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE usuario 
                SET Nombre = @Nombre, 
                    Telefono = @Telefono, 
                    IDPerfil = @IDPerfil
                WHERE IDUsuario = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new 
            { 
                usuario.Nombre,
                usuario.Telefono,
                usuario.IDPerfil,
                usuario.Id 
            });

            return affectedRows > 0 
                ? Result.Success() 
                : Result.Failure("Error al actualizar usuario");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "UPDATE usuario SET Activo = 0 WHERE IDUsuario = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });

            return affectedRows > 0 
                ? Result.Success() 
                : Result.Failure("Error al eliminar usuario");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error de base de datos: {ex.Message}");
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM usuario WHERE Email = @Email";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.ToLowerInvariant() });
            return count > 0;
        }
        catch
        {
            return false;
        }
    }
}
