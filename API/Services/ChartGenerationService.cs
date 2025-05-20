using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using OpenAI.Assistants;

namespace AswinsIntelligence.Services
{
    public class ChartGenerationService : IChartGenerationService
    {
        private readonly IConfiguration _configuration;
        private readonly OpenAIClient _openAiClient;
        private readonly AssistantClient _assistantClient;
        private string _assistantId;

        public ChartGenerationService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Create the OpenAI client
            _openAiClient = OpenAIAssistantAgent.CreateOpenAIClient(new ApiKeyCredential(configuration["OpenAi:ApiKey"]));

            // Initialize the assistant client
            _assistantClient = _openAiClient.GetAssistantClient();
        }

        [Experimental("OPENAI001")]
        public string GenerateChart(string jsonData)
        {
            var assistantClient = _openAiClient.GetAssistantClient();

            var assistant =
                assistantClient.CreateAssistantAsync(
                    "gpt-4-turbo",
                    name: "Chart expert",
                    instructions:
                    """
                    Generate a visually appealing chart from the JSON data. Use matplotlib or another appropriate visualization library. Include clear labels, a title, and appropriate colors.
                    """,
                    enableCodeInterpreter: true).GetAwaiter().GetResult();

// Create agent
            OpenAIAssistantAgent agent = new(assistant, assistantClient);

            AgentThread thread = new OpenAIAssistantAgentThread(_assistantClient);
            ChatMessageContent message = new(AuthorRole.User,
                $@"Create a visualization based on this JSON data:
{jsonData}

Please generate an appropriate chart based on the data. Use matplotlib or plotly.
Save the chart as a PNG file.
Do not return any text or code explanation."
            );

            return InvokeAgentAsync(message, agent).GetAwaiter().GetResult();
        }

        async Task<string> InvokeAgentAsync(ChatMessageContent message, OpenAIAssistantAgent agent)
        {
            await foreach (ChatMessageContent response in agent.InvokeAsync(message))
            {
                foreach (var item in response.Items)
                {
                    if (item is FileReferenceContent content)
                    {
                        var fileId = content.FileId;
                        if (!string.IsNullOrEmpty(fileId))
                        {
                            var fileContent = _openAiClient.GetOpenAIFileClient().DownloadFileAsync(fileId).GetAwaiter().GetResult();
                            using (var memoryStream = new MemoryStream())
                            {
                                fileContent.Value.ToStream().CopyTo(memoryStream);
                                return Convert.ToBase64String(memoryStream.ToArray());
                            }
                        }
                    }
                }
            }

            Console.WriteLine("No image was found in the response");
            return null;
        }

        private string ExtractFileIdFromContent(string content)
        {
            // This is a simplified example - you might need more sophisticated parsing
            // depending on how the file ID is included in the response
            if (content.Contains("fileId:"))
            {
                var startIndex = content.IndexOf("fileId:") + 7;
                var endIndex = content.IndexOf(",", startIndex);
                if (endIndex == -1) endIndex = content.IndexOf("}", startIndex);
                if (endIndex == -1) endIndex = content.Length;

                return content.Substring(startIndex, endIndex - startIndex).Trim(' ', '"');
            }

            return null;
        }
    }
}