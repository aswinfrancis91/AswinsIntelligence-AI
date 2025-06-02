# AswinsIntelligence AI

AswinsIntelligence is a learning project that demonstrates how to build AI-powered applications using both Microsoft's
Semantic Kernel and direct API calls to various AI services. The project integrates multiple AI capabilities including
natural language to SQL conversion, conversation management, and image generation.

## Overview

This project serves as a learning platform for working with AI technologies, specifically showing:

1. How to use Microsoft's Semantic Kernel for structured AI interactions
2. How to make direct API calls to AI services when needed
3. Integration of multiple AI models (local and cloud-based)

## Features

- **Natural Language to SQL Conversion**: Convert plain English questions into SQL queries that can be executed against
  a database
- **Conversation Management**: Maintain conversation context across multiple interactions
- **Image Generation**: Generate visual representations of data using both:
    - Chart generation via Semantic Kernel's OpenAI integration
    - DALL-E image generation via direct API calls

- **Multi-Model Support**: Use different AI models including:
    - Local LLM via Ollama (deepseek-r1)
    - OpenAI's GPT-4 Turbo

## Technical Stack

- **.NET 9.0**: Built using the latest .NET framework
- **ASP.NET Core**: Web API endpoints for various AI functions
- **C# 13.0**: Uses the latest C# language features
- **Semantic Kernel**: For structured AI interactions
- **OpenAI SDK**: For direct API interactions with OpenAI services
- **.NET Aspire**: For service orchestration, observability, and resilience
- **Scalar**: For API documentation and reference
- **React**: Frontend application with chat interface

## Architecture

The project is structured around services and interfaces that abstract the AI functionality:

- **INlToSqlService**: Converts natural language to SQL queries
- **IDbService**: Executes the generated SQL queries
- **IConversationService**: Manages conversation state
- **IImageGenerationService**: Generates visual representations (charts and images)

## AI Integration Approaches

This project demonstrates two approaches to working with AI services:

### 1. Semantic Kernel Integration

The project uses Microsoft's Semantic Kernel framework for structured AI interactions. This provides:

- Type-safe interfaces to AI services
- Conversation and context management
- Simplified agent creation and interaction

Example (from ): `ImageGenerationService.cs`

```csharp 
// Using Semantic Kernel for chart generation 
OpenAIAssistantAgent agent = new(assistant, assistantClient); 
AgentThread thread = new OpenAIAssistantAgentThread(_assistantClient); 
ChatMessageContent message = new(AuthorRole.User, "...");
``` 

### 2. Direct API Calls

For some functionality, the project makes direct API calls to AI services using HttpClient, showing a more hands-on
approach:
Example (from ): `ImageGenerationService.cs`

```csharp 
// Direct API call to DALL-E 
var client = new HttpClient(); 
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["OpenAi:ApiKey"]); 
var response = client.PostAsync("https://api.openai.com/v1/images/generations)", content).GetAwaiter().GetResult();
``` 

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Access to OpenAI API (API key required)
- Ollama running locally (for local LLM support)
- React frontend running on [http://localhost:5173](http://localhost:5173) (for the full experience)

### Configuration

The application uses `appsettings.json` for configuration. You'll need to set:

- OpenAI API key
- Ollama endpoint (for local LLM)
- SQL Server connection string (for database queries)

### Running the Application

1. Clone the repository
2. Configure your API keys in `appsettings.json`
3. Run the application using .NET Aspire: `dotnet run --project AppHost`
4. The Aspire dashboard will open, providing monitoring and telemetry for your application
5. The API will be available at [http://localhost:5257/scalar/v1](http://localhost:5257/scalar/v1)
6. Launch the React frontend application

## .NET Aspire Integration

The project has been enhanced with .NET Aspire integration, which provides:

- **Service Orchestration**: Simplified coordination of multiple services
- **Observability**: Built-in logging, metrics, and distributed tracing
- **Resilience**: HTTP resilience and service discovery
- **Health Checks**: Automated health monitoring of services
- **Telemetry**: OpenTelemetry integration for application insights

The application now uses:

- **Structured Logging**: Enhanced logging through Serilog with OpenTelemetry integration
- **Service Defaults**: Common service configurations through Aspire's ServiceDefaults
- **SQL Server Client**: Simplified database connections with resilience built-in

## API Endpoints

- `/AskAswin`: Convert natural language to SQL, execute the query, and return results
- `/ResetConversation`: Reset the conversation context for a specific user
- `/GenerateGraphDallE`: Generate data visualizations using DALL-E
- `/GenerateGraph`: Generate charts based on data using AI

## Frontend Interface

The React application provides a user-friendly interface with:

- **Chat Window**: Main interface for interacting with the AI
- **Function Buttons**:
    1. **Ask Aswin**: Submit questions to be converted to SQL and executed
    2. **Generate Chart**: Create visualizations from data using Semantic Kernel
    3. **Generate DALL-E Image**: Create visualizations using DALL-E direct API
    4. **Reset Conversation**: Clear the current conversation history

## Using the Application

1. Start the application using .NET Aspire: `dotnet run --project AppHost`
2. Monitor the application through the Aspire dashboard
3. Launch the React frontend (runs on [http://localhost:5173](http://localhost:5173))
4. Use the chat window to interact with the AI
5. Use the function buttons to trigger specific AI capabilities:

- Ask questions about your data using natural language
- Generate visual representations of your data
- Reset the conversation when needed

## Learning Resources

This project is designed for learning AI integration techniques. Here are some areas to explore:

- Comparing Semantic Kernel vs direct API approaches
- Understanding AI model differences (local vs cloud)
- Strategies for managing conversation context
- Techniques for generating and processing AI-created visuals
- .NET Aspire for cloud-native application development

## License

This project is intended for learning purposes. Please check the repository for license information.
