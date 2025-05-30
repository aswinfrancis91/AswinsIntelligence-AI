namespace API.Interfaces;

public interface IImageGenerationService
{
    /// <summary>
    /// Generates a chart based on the provided JSON data.
    /// The chart is created using a visualization library such as matplotlib or plotly and saved as a PNG file.
    /// </summary>
    /// <param name="jsonData">The JSON-formatted data used to generate the chart.</param>
    /// <returns>A string representing the outcome or file path of the generated chart, or null if the operation fails.</returns>
    string? GenerateChart(string jsonData);
    
    /// <summary>
    /// Generates a graph or chart as a base64-encoded PNG image based on the provided JSON data.
    /// </summary>
    /// <param name="question">The JSON data and relevant user question to generate a visualization from.</param>
    /// <returns>A base64-encoded string representing the PNG image of the generated graph or chart.</returns>
    string? GenerateDalleImage(string question);
}
