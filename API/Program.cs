using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.SemanticKernel;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
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

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["service.name"] = "AswinsIntelligence"
        };
    })
);
builder.AddSqlServerClient(connectionName: "DefaultConnection");
var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"]);
    };
});


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.MapPost("/AskAswin", (string question, INlToSqlService nlToSqlService, IDbService dbService, ILogger<Program> logger, string userId = "default", AIModels model = AIModels.DeepseekR1) =>
    {
        logger.LogInformation("Received query: {Question} from user: {UserId} using model: {Model}", question, userId, model);
        var result = nlToSqlService.GenerateSqlQuery(question, model, userId);
        result.DbResult = dbService.ExecuteQuery(result.SqlQuery);
        logger.LogInformation("Successfully executed SQL query and returned {RowCount} rows", result.DbResult.Length);

        return result;
    })
    .WithName("GetAnswers");

app.MapPost("/ResetConversation", (string userId, IConversationService conversationService) =>
    {
        conversationService.ResetConversation(userId ?? "default");
        return Results.Ok(new { message = "Conversation reset successfully" });
    })
    .WithName("ResetConversation");

app.MapPost("/GenerateGraphDallE", (string question, IImageGenerationService imageGenerationService, ILogger<Program> logger) =>
    {
        logger.LogInformation("Generating DALL-E image for question: {Question}", question);
        var result = imageGenerationService.GenerateDalleImage(question);
        logger.LogInformation("Successfully generated DALL-E image");

        return result;
    })
    .WithName("GetDallEChart");

app.MapPost("/GenerateGraph", (string question, IImageGenerationService imageGenerationService, ILogger<Program> logger) =>
    {
        logger.LogInformation("Generating chart for question: {Question}", question);
        var result = imageGenerationService.GenerateChart(question);
        logger.LogInformation("Successfully generated chart");

        return result;
    })
    .WithName("GetChart");

app.Run();