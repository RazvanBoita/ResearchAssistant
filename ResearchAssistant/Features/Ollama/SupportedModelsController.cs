using Microsoft.AspNetCore.Mvc;

namespace ResearchAssistant.Features.Ollama;


[ApiController]
[Route("api/models")]
public class SupportedModelsController : ControllerBase
{
    private readonly List<string> _supportedModels;

    public SupportedModelsController(IConfiguration configuration)
    {
        _supportedModels = configuration.GetSection("SupportedModels").Get<List<string>>() ?? [];
    }
    [HttpGet]
    public ActionResult<List<string>> GetModels()
    {
        return Ok(_supportedModels);
    }
}