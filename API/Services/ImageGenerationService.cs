using System.ClientModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AswinsIntelligence.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using OpenAI.Assistants;

namespace AswinsIntelligence.Services
{
    /// <summary>
    /// Service responsible for generating visual representations, including custom charts and DALL-E generated images,
    /// utilizing AI capabilities such as OpenAI's GPT and related visualization libraries.
    /// Chart Generation shows example code connecting to OpenAI using semantic kernel's open AI wrapper
    /// Image generation using Dall-E uses the api directly
    /// </summary>
    public class ImageGenerationService : IImageGenerationService
    {
        private readonly IConfiguration _configuration;
        private readonly OpenAIClient _openAiClient;
        private readonly AssistantClient _assistantClient;

        public ImageGenerationService(IConfiguration configuration)
        {
            _configuration = configuration;

            _openAiClient = OpenAIAssistantAgent.CreateOpenAIClient(new ApiKeyCredential(configuration["OpenAi:ApiKey"]));

            _assistantClient = _openAiClient.GetAssistantClient();
        }

        /// <summary>
        /// Generates a visual chart representation based on the provided JSON data.
        /// </summary>
        /// <param name="jsonData">A string containing the JSON-structured data to be visualized as a chart.</param>
        /// <returns>A Base64-encoded string representing the PNG file of the generated chart, or null if chart generation fails.</returns>
        public string? GenerateChart(string jsonData)
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

            OpenAIAssistantAgent agent = new(assistant, assistantClient);

            AgentThread thread = new OpenAIAssistantAgentThread(_assistantClient);
            ChatMessageContent message = new(AuthorRole.User,
                $"""
                 Create a visualization based on this JSON data:
                 {jsonData}

                 Please generate an appropriate chart based on the data. Use matplotlib or plotly.
                 Save the chart as a PNG file.
                 Do not return any text or code explanation.
                 """
            );

            return InvokeAgentAsync(message, agent).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Invokes the AI agent with a given chat message and processes the response to return a Base64-encoded string
        /// representation of a file, if available in the response.
        /// </summary>
        /// <param name="message">The chat message contents to be sent to the AI agent.</param>
        /// <param name="agent">The AI agent instance responsible for processing the provided message.</param>
        /// <returns>A Base64-encoded string of the file content if a file is included in the response; otherwise, null.</returns>
        private async Task<string?> InvokeAgentAsync(ChatMessageContent message, OpenAIAssistantAgent agent)
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

        /// <inheritdoc/>
        public string GenerateDalleImage(string question)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["OpenAi:ApiKey"]);

            var requestBody = new
            {
                model = "dall-e-3",
                prompt = $@"
You are a data visualization expert. Your task is to create a chart or graph based on the provided JSON data. The chart should be clear, insightful, and directly address the user's question.
You must:
1. Generate a base64-encoded PNG image of the visualization
2. Return ONLY the base64-encoded image data with the proper data URI prefix (data:image/png;base64,)

JSON: {question}
",
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
    }
}