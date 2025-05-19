using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AswinsIntelligence.Services;

public class DbService(IConfiguration configuration) : IDbService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                ?? throw new ArgumentNullException(nameof(_connectionString),
                                                    "Database connection string is missing in configuration");

    /// <inheritdoc />
    public string ExecuteQuery(string sqlQuery)
    {
        try
        {
            var results = ExecuteQueryToDynamic(sqlQuery);
            return JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            var error = new { Error = $"Error executing query: {ex.Message}", SqlQuery = sqlQuery };
            return JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    public IEnumerable<dynamic> ExecuteQueryToDynamic(string sqlQuery)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var results = connection.Query(sqlQuery, commandTimeout: 60);
        return results;
    }
}