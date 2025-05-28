using System.Text.Json;
using AswinsIntelligence.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AswinsIntelligence.Services;

/// <summary>
/// Provides database-related services and functionalities, including executing SQL queries and processing results.
/// </summary>
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

    /// <summary>
    /// Executes the provided SQL query and returns the results as a collection of dynamic objects.
    /// </summary>
    /// <param name="sqlQuery">The SQL query to execute against the database.</param>
    /// <returns>An enumerable collection of dynamic objects representing the query results.</returns>
    private IEnumerable<dynamic> ExecuteQueryToDynamic(string sqlQuery)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var results = connection.Query(sqlQuery, commandTimeout: 60);
        return results;
    }
}