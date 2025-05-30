using System.Text.Json;
using API.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace API.Services;

/// <summary>
/// Provides database-related services and functionalities, including executing SQL queries and processing results.
/// </summary>
public class DbService(IConfiguration configuration, ILogger<DbService> logger,SqlConnection sqlConnection) : IDbService
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                ?? throw new ArgumentNullException(nameof(_connectionString),
                                                    "Database connection string is missing in configuration");

    /// <inheritdoc />
    public string ExecuteQuery(string sqlQuery)
    {
        logger.LogInformation("Executing SQL query, length: {QueryLength} characters", sqlQuery?.Length ?? 0);
        logger.LogDebug("SQL Query: {SqlQuery}", sqlQuery);

        try
        {
            logger.LogDebug("Attempting to execute query against database");
            var results = ExecuteQueryToDynamic(sqlQuery);

            logger.LogDebug("Query executed successfully, serializing results");
            var serializedResult = JsonSerializer.Serialize(results, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            logger.LogInformation("Query executed and results serialized successfully. Result length: {ResultLength} characters",
                serializedResult?.Length ?? 0);

            return serializedResult;
        }
        catch (SqlException sqlEx)
        {
            logger.LogError(sqlEx, "SQL error executing query: {ErrorMessage}, Error Number: {ErrorNumber}, Severity: {Severity}",
                sqlEx.Message, sqlEx.Number, sqlEx.Class);

            var error = new { Error = $"SQL Error: {sqlEx.Message}", SqlQuery = sqlQuery, ErrorNumber = sqlEx.Number };
            return JsonSerializer.Serialize(error, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing query: {ErrorMessage}", ex.Message);

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
        logger.LogDebug("Creating SQL connection");

        using var connection = sqlConnection;
        try
        {
            connection.Open();
            var results = connection.Query(sqlQuery, commandTimeout: 60);

            var resultCount = results?.Count() ?? 0;
            logger.LogInformation("Query execution completed, returned {RowCount} rows", resultCount);

            return results;
        }
        catch (SqlException sqlEx)
        {
            logger.LogError(sqlEx, "SQL exception while executing query. Error: {ErrorMessage}, State: {State}",
                sqlEx.Message, sqlEx.State);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while executing query or processing results");
            throw;
        }
        finally
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                logger.LogDebug("Closing database connection");
                connection.Close();
            }
        }
    }
}