using AswinsIntelligence.Interfaces;
using AswinsIntelligence.Models;
using AswinsIntelligence.Services;
using Microsoft.SemanticKernel;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

#region Local LLM

var client = new HttpClient();
client.Timeout = TimeSpan.FromMinutes(60);
client.BaseAddress = new Uri(builder.Configuration["Ollama:Endpoint"]);

builder.Services.AddOllamaChatCompletion(modelId: "deepseek-r1:latest", httpClient: client);

#endregion

builder.Services.AddSingleton<INlToSqlService, NlToSqlService>();
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddSingleton<IConversationService, ConversationService>();
builder.Services.AddSingleton<IImageGenerationService, ImageGenerationService>();

builder.Services.AddOpenAIChatCompletion(modelId: "gpt-4-turbo", apiKey: builder.Configuration["OpenAi:ApiKey"]);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.MapPost("/AskAswin", (string question, INlToSqlService nlToSqlService, IDbService dbService, string userId = "default", AIModels model = AIModels.DeepseekR1) =>
    {
        var result = nlToSqlService.GenerateSqlQuery(question, model, userId);
        result.DbResult = dbService.ExecuteQuery(result.SqlQuery);

        return result;
    })
    .WithName("GetAnswers");

app.MapPost("/ResetConversation", (string userId, IConversationService conversationService) =>
    {
        conversationService.ResetConversation(userId ?? "default");
        return Results.Ok(new { message = "Conversation reset successfully" });
    })
    .WithName("ResetConversation");

app.MapPost("/GenerateGraphDallE", (string question, IImageGenerationService imageGenerationService, IDbService dbService) =>
    {
        var result = imageGenerationService.GenerateDalleImage(question);
        return result;
    })
    .WithName("GetDallEChart");

app.MapPost("/GenerateGraph", (string question, IImageGenerationService imageGenerationService) =>
    {
        var result = imageGenerationService.GenerateChart(question);
        return result;
    })
    .WithName("GetChart");

app.Run();