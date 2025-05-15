using System.Reflection;
using AswinsIntelligence.Models;
using Microsoft.SemanticKernel.ChatCompletion;

// ReSharper disable InconsistentNaming

namespace AswinsIntelligence.Services;

public class NlToSqlService(IChatCompletionService chatCompletionService) : INlToSqlService
{
    /// <inheritdoc />
    public ApiResult GenerateSqlQuery(string question, AIModels model)
    {
        if (model == AIModels.DeepseekR1)
        {
            return ProcessWithLocalLlm(question);
        }

        return ProcessWithOpenAI(question);
    }

    private ApiResult ProcessWithOpenAI(string question)
    {
        return null;
    }

    private ApiResult ProcessWithLocalLlm(string question)
    {
        var chatHistory = new ChatHistory();
        var prompt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Prompts", "SystemMessage.txt"));

        chatHistory.AddSystemMessage(prompt);
        chatHistory.AddUserMessage($"Generate a SQL query for Microsoft SQL Server that answers this question:{question}. Only return the SQL query without any explanations.");

        var reply = chatCompletionService.GetChatMessageContentAsync(chatHistory).GetAwaiter().GetResult();
        var replyContent = reply.Content ?? string.Empty;
        chatHistory.AddMessage(reply.Role, replyContent);

        return ParseLlmResponse(replyContent);
    }

    /// <summary>
    /// Parses the response from the language model to extract SQL query and additional details.
    /// </summary>
    /// <param name="response">The raw response string from the language model.</param>
    /// <returns>A <see cref="ApiResult"/> object containing parsed thoughts and SQL query.</returns>
    private ApiResult ParseLlmResponse(string response)
    {
        var result = new ApiResult();

        if (response.Contains("<think>") && response.Contains("</think>"))
        {
            int thinkStart = response.IndexOf("<think>") + "<think>".Length;
            int thinkEnd = response.IndexOf("</think>");

            if (thinkEnd > thinkStart)
            {
                result.Thoughts = response.Substring(thinkStart, thinkEnd - thinkStart).Trim();

                if (thinkEnd + "</think>".Length < response.Length)
                {
                    result.SqlQuery = response.Substring(thinkEnd + "</think>".Length).Replace("sql", "").Replace("```", "").Trim();
                }
            }
        }
        else
        {
            result.SqlQuery = response.Trim();
        }

        return result;
    }
}