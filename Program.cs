using AswinsIntelligence.Models;
using AswinsIntelligence.Services;
using Microsoft.SemanticKernel;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var client = new HttpClient();
client.Timeout = TimeSpan.FromMinutes(60);
client.BaseAddress = new Uri("http://localhost:11434");

builder.Services.AddOllamaChatCompletion(modelId: "deepseek-r1:latest", httpClient: client);
builder.Services.AddSingleton<INlToSqlService, NlToSqlService>();
builder.Services.AddScoped<IDbService, DbService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();


app.MapPost("/AskAswin", (string question, INlToSqlService nlToSqlService, IDbService dbService, AIModels model = AIModels.DeepseekR1) =>
    {
        var result = nlToSqlService.GenerateSqlQuery(question, model);
        result.DbResult = dbService.ExecuteQuery(result.SqlQuery);
        return result;
    })
    .WithName("GetAnswers");

app.Run();