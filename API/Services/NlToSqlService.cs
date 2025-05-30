using API.Interfaces;
using API.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace API.Services;

public class NlToSqlService(IChatCompletionService chatCompletionService, IConversationService conversationService, ILogger<NlToSqlService> logger) : INlToSqlService
{
    /// <inheritdoc />
    public ApiResult GenerateSqlQuery(string question, AIModels model, string userId = "default")
    {
        logger.LogInformation("Generating SQL query for question: '{Question}' using model: {Model}, userId: {UserId}", question, model, userId);

        if (model == AIModels.DeepseekR1)
        {
            logger.LogDebug("Using local LLM (DeepseekR1) for processing");
            return ProcessWithLocalLlm(question);
        }

        logger.LogDebug("Using OpenAI for processing");
        return ProcessWithOpenAI(question, userId);
    }

    /// <summary>
    /// Processes a natural language question using OpenAI to generate a corresponding SQL query.
    /// </summary>
    /// <param name="question">The natural language question to be converted into a SQL query.</param>
    /// <param name="userId">The unique identifier of the user initiating the query. Defaults to "default".</param>
    /// <returns>An <see cref="ApiResult"/> containing the generated SQL query.</returns>
    private ApiResult ProcessWithOpenAI(string question, string userId)
    {
        try
        {
            var chatHistory = conversationService.GetOrCreateConversation(userId);

            // Only add system message if this is a new conversation
            if (chatHistory.Count == 0)
            {
                logger.LogDebug("New conversation detected, adding system prompt");
                var prompt = File.ReadAllText(Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Prompts", "NlToSql.txt"));
                chatHistory.AddSystemMessage(prompt);
            }

            chatHistory.AddSystemMessage("It should output only one SQL query and so if the questions are multiple the SQL query has to be single one");
            chatHistory.AddUserMessage($"Generate a SQL query for Microsoft SQL Server that answers this question:{question}.");

            logger.LogInformation("Sending request to OpenAI for SQL generation");
            var reply = chatCompletionService.GetChatMessageContentAsync(chatHistory).GetAwaiter().GetResult();
            logger.LogDebug("Received response from OpenAI, content length: {Length}", reply.Content?.Length);
            logger.LogInformation("Response: {@Response}", reply);
            var replyContent = reply.Content ?? string.Empty;
            chatHistory.AddMessage(reply.Role, replyContent);


            var sqlQuery = replyContent.Replace("sql", "").Replace("`", "").Trim();

            logger.LogInformation("Successfully generated SQL query using OpenAI. SQL Query: {SqlQuery}",
                sqlQuery);

            return new ApiResult
            {
                SqlQuery = sqlQuery
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing question with OpenAI: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    #region Ollama

    /// <summary>
    /// Processes the specified question using a local language model to generate a SQL query.
    /// </summary>
    /// <param name="question">The question for which the SQL query should be generated.</param>
    /// <returns>An <see cref="ApiResult"/> object containing the generated SQL query and relevant details.</returns>
    private ApiResult ProcessWithLocalLlm(string question)
    {
        try
        {
            logger.LogInformation("Processing question with local LLM (Ollama)");


            var chatHistory = new ChatHistory();
            var prompt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Prompts", "NlToSql.txt"));

            chatHistory.AddSystemMessage(prompt);
            chatHistory.AddUserMessage($"Generate a SQL query for Microsoft SQL Server that answers this question:{question}.");

            var reply = chatCompletionService.GetChatMessageContentAsync(chatHistory).GetAwaiter().GetResult();
            logger.LogDebug("Received response from Local LLM, content length: {Length}", reply.Content?.Length);

            var replyContent = reply.Content ?? string.Empty;
            chatHistory.AddMessage(reply.Role, replyContent);

            var result = ParseLlmResponseForOllama(replyContent);

            logger.LogInformation("Successfully generated SQL query using local LLM. Query: {Query}, Thoughts: {Thoughts}",
                result.SqlQuery, result.Thoughts);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing question with local LLM: {ErrorMessage}", ex.Message);
            throw;
        }
    }


    /// <summary>
    /// Parses the response from the language model to extract SQL query and additional details.
    /// </summary>
    /// <param name="response">The raw response string from the language model.</param>
    /// <returns>A <see cref="ApiResult"/> object containing parsed thoughts and SQL query.</returns>
    private ApiResult ParseLlmResponseForOllama(string response)
    {
        logger.LogDebug("Parsing LLM response of length: {Length}", response.Length);

        var result = new ApiResult();

        if (response.Contains("<think>") && response.Contains("</think>"))
        {
            logger.LogTrace("Found thinking tags in response");

            int thinkStart = response.IndexOf("<think>") + "<think>".Length;
            int thinkEnd = response.IndexOf("</think>");

            if (thinkEnd > thinkStart)
            {
                result.Thoughts = response.Substring(thinkStart, thinkEnd - thinkStart).Trim();
                logger.LogDebug("Extracted thoughts from response, length: {ThoughtsLength}", result.Thoughts.Length);

                if (thinkEnd + "</think>".Length < response.Length)
                {
                    result.SqlQuery = response.Substring(thinkEnd + "</think>".Length).Replace("sql", "").Replace("```", "").Trim();
                    logger.LogDebug("Extracted SQL query from response, length: {QueryLength}", result.SqlQuery.Length);
                }
                else
                {
                    logger.LogWarning("No SQL query found after thoughts section");
                }
            }
            else
            {
                logger.LogWarning("Invalid think tags format in response");
            }
        }
        else
        {
            logger.LogDebug("No thinking tags found, treating entire response as SQL query");
            result.SqlQuery = response.Trim();
        }

        return result;
    }

    #endregion
}