namespace API.Interfaces;

public interface IDbService
{
    /// <summary>
    /// Executes the provided SQL query and returns the result serialized as a JSON string.
    /// </summary>
    /// <param name="sqlQuery">The SQL query to be executed.</param>
    /// <returns>A JSON-formatted string representing the query results or an error message if the execution fails.</returns>
    string ExecuteQuery(string sqlQuery);
}