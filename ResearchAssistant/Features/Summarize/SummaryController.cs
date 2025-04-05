using Microsoft.AspNetCore.Mvc;
using OllamaSharp;
using ResearchAssistant.Features.PdfParsing;

namespace ResearchAssistant.Features.Summarize;

[ApiController]
[Route("api/summary")]
public class SummaryController : ControllerBase
{
    private readonly Uri _uri = new Uri("http://localhost:11434");
    private OllamaApiClient _ollamaApiClient;
    private readonly ChunkingService _chunkingService;
    private readonly SummarizationService _summarizationService;

    public SummaryController(ChunkingService chunkingService, SummarizationService summarizationService)
    {
        _chunkingService = chunkingService;
        _summarizationService = summarizationService;
        _ollamaApiClient = new OllamaApiClient(_uri);
        _ollamaApiClient.SelectedModel = "phi";
    }
    
    [HttpPost]
    public async Task Summarize([FromBody] SummaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            HttpContext.Response.StatusCode = 400;
            await HttpContext.Response.WriteAsync("Text cannot be empty.");
            return;
        }

        HttpContext.Response.ContentType = "text/plain";
        var chunks = _chunkingService.SplitIntoChunks(request.Text, 300);

        foreach (var chunk in chunks)
        {
            Console.WriteLine("Processing a chunk");
        
        
            await foreach (var stream in _ollamaApiClient.GenerateAsync($"Make a very short summary for this: {chunk}"))
            {
                await HttpContext.Response.WriteAsync(stream.Response);
                await HttpContext.Response.Body.FlushAsync();
            }
            
            await HttpContext.Response.Body.FlushAsync();
            Console.WriteLine("Finished processing a chunk");
        }
    }
    
}