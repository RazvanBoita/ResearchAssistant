using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using OllamaSharp;

namespace ResearchAssistant.Features.Ollama;

[ApiController]
[Route("api/ollama")]
public class OllamaController : ControllerBase
{
    private readonly Uri _uri = new Uri("http://localhost:11434");
    private OllamaApiClient _ollamaApiClient;

    public OllamaController()
    {
        _ollamaApiClient = new OllamaApiClient(_uri);
    }
    [HttpPost]
    public async Task<IActionResult> AskQuestion([FromBody] IncomingQuestionRequest question)
    {
        try
        {
            _ollamaApiClient.SelectedModel = question.Model;
            Console.WriteLine("Model being used is: " + question.Model);
        
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");
        
        

            await foreach (var chunk in _ollamaApiClient.GenerateAsync(question.Question))
            {
                await Response.WriteAsync(chunk.Response);
                await Response.Body.FlushAsync();
            }

            return new EmptyResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.ToString());
            throw;
        }
    }
    
}