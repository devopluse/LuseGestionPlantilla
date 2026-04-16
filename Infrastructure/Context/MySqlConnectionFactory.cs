using MySqlConnector;
using System.Data;

namespace MyBackendApi.Infrastructure.Context;

public interface IMySqlConnectionFactory
{
    IDbConnection CreateConnection();
}

public class MySqlConnectionFactory : IMySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
