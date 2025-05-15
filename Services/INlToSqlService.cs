using AswinsIntelligence.Models;

namespace AswinsIntelligence.Services;

public interface INlToSqlService
{
    /// <summary>
    /// Generates a SQL query based on the provided natural language question.
    /// </summary>
    /// <param name="question">The natural language question for which a SQL query is to be generated.</param>
    /// <param name="model"></param>
    /// <returns>A <see cref="ApiResult"/> object containing the generated SQL query, thoughts, and any additional response details.</returns>
    ApiResult GenerateSqlQuery(string question, AIModels model, string userId);
}