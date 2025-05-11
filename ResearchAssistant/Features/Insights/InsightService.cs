using OllamaSharp;
using ResearchAssistant.Features.PdfParsing;

namespace ResearchAssistant.Features.Insights;

public class InsightService
{
    private readonly Uri _uri = new Uri("http://localhost:11434");
    private readonly OllamaApiClient _ollamaApiClient;
    private readonly ChunkingService _chunkingService;
    private readonly List<string> _supportedModels;
    public InsightService(ChunkingService chunkingService, IConfiguration configuration)
    {
        _chunkingService = chunkingService;
        _ollamaApiClient = new OllamaApiClient(_uri);
        _ollamaApiClient.SelectedModel = "phi"; //default model
        _supportedModels = configuration.GetSection("SupportedModels").Get<List<string>>() ?? [];
    }

    public async Task ExtractInsights(HttpContext httpContext, InsightRequest request)
    {
        string text = request.Text ?? throw new ArgumentNullException(nameof(request.Text));
        
        if (string.IsNullOrWhiteSpace(text))
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Text cannot be empty.");
            return;
        }
        if (!string.IsNullOrWhiteSpace(request.Model))
        {
            if (!_supportedModels.Contains(request.Model))
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Model not supported.");
                return;
            }
            _ollamaApiClient.SelectedModel = request.Model;
        }
        httpContext.Response.ContentType = "text/plain";
        var chunks = _chunkingService.SplitIntoChunks(text, 400);
        // var stopper = 0;
        foreach (var chunk in chunks)
        {
            // stopper++;
            // if(stopper == 4) break;
            Console.WriteLine("Processing a chunk");
            await foreach (var stream in _ollamaApiClient.GenerateAsync($"Extract all the key insights from this text: {chunk}"))
            {
                await httpContext.Response.WriteAsync(stream.Response);
                await httpContext.Response.Body.FlushAsync();
            }

            await httpContext.Response.Body.FlushAsync();
            Console.WriteLine("Finished processing a chunk");
        }
    }
}