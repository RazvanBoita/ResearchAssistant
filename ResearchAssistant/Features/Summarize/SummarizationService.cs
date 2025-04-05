using System.Text;
using OllamaSharp;

namespace ResearchAssistant.Features.Summarize;

public class SummarizationService
{
    private readonly Uri _uri = new Uri("http://localhost:11434");
    private OllamaApiClient _ollamaApiClient;
    
    public SummarizationService()
    {
        _ollamaApiClient = new OllamaApiClient(_uri);
        _ollamaApiClient.SelectedModel = "phi";
    }

    public async Task<string> SummarizeAsync(string text)
    {
        var prompt = $"Make a summary for: {text}";
        var result = new StringBuilder();
        await foreach (var stream in _ollamaApiClient.GenerateAsync(prompt))
            result.Append(stream.Response);
        return result.ToString();
    }
}