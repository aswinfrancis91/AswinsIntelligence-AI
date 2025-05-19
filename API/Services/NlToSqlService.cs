using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using AswinsIntelligence.Models;
using Microsoft.SemanticKernel.ChatCompletion;

// ReSharper disable InconsistentNaming

namespace AswinsIntelligence.Services;

public class NlToSqlService(IChatCompletionService chatCompletionService,IConversationService conversationService,IConfiguration configuration) : INlToSqlService
{
    /// <inheritdoc />
    public ApiResult GenerateSqlQuery(string question, AIModels model,string userId = "default")
    {
        if (model == AIModels.DeepseekR1)
        {
            return ProcessWithLocalLlm(question);
        }

        return ProcessWithOpenAI(question,userId);
    }

    public string GenerateGraph(string question)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["OpenAi:ApiKey"]);

        var requestBody = new
        {
            model = "dall-e-3",
            prompt = $@"
You are a data visualization expert. Your task is to create a chart or graph based on the provided JSON data. The chart should be clear, insightful, and directly address the user's question.
You must:
1. Generate a base64-encoded PNG image of the visualization
2. Return ONLY the base64-encoded image data with the proper data URI prefix (data:image/png;base64,)

JSON: {question}
"
            ,
            n = 1,
            size = "1024x1024",
            response_format = "b64_json"
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = client.PostAsync("https://api.openai.com/v1/images/generations", content).GetAwaiter().GetResult();
        var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var json = JsonDocument.Parse(responseString);
        var base64Image = json.RootElement.GetProperty("data")[0].GetProperty("b64_json").GetString();

        return base64Image;
    }
    private ApiResult ProcessWithOpenAI(string question,string userId)
    {
        var chatHistory = conversationService.GetOrCreateConversation(userId);
        
        // Only add system message if this is a new conversation
        if (chatHistory.Count == 0)
        {
            var prompt = File.ReadAllText(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                "Prompts", "NlToSql.txt"));
            chatHistory.AddSystemMessage(prompt);
        }
        chatHistory.AddSystemMessage("It should output only one SQL query and so if the questions are multiple the SQL query has to be single one");
        chatHistory.AddUserMessage($"Generate a SQL query for Microsoft SQL Server that answers this question:{question}.");
        
        var reply = chatCompletionService.GetChatMessageContentAsync(chatHistory).GetAwaiter().GetResult();
        var replyContent = reply.Content ?? string.Empty;
        chatHistory.AddMessage(reply.Role, replyContent);


        return new ApiResult
        {
            SqlQuery = replyContent.Replace("sql", "").Replace("`", "").Trim()
        };

    }

    private ApiResult ProcessWithLocalLlm(string question)
    {
        var chatHistory = new ChatHistory();
        var prompt = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Prompts", "NlToSql.txt"));

        chatHistory.AddSystemMessage(prompt);
        chatHistory.AddUserMessage($"Generate a SQL query for Microsoft SQL Server that answers this question:{question}.");

        var reply = chatCompletionService.GetChatMessageContentAsync(chatHistory).GetAwaiter().GetResult();
        var replyContent = reply.Content ?? string.Empty;
        chatHistory.AddMessage(reply.Role, replyContent);

        return ParseLlmResponseForOllama(replyContent);
    }

 
    /// <summary>
    /// Parses the response from the language model to extract SQL query and additional details.
    /// </summary>
    /// <param name="response">The raw response string from the language model.</param>
    /// <returns>A <see cref="ApiResult"/> object containing parsed thoughts and SQL query.</returns>
    private ApiResult ParseLlmResponseForOllama(string response)
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