using Microsoft.AspNetCore.Mvc;
using ResearchAssistant.Features.PdfParsing.Text7;

namespace ResearchAssistant.Features.Insights;

[ApiController]
[Route("api/insights")]
public class InsightController : ControllerBase
{
    private readonly InsightService _insightService;
    private readonly TextSevenPdfService _pdfService;

    public InsightController(InsightService insightService, TextSevenPdfService pdfService)
    {
        _insightService = insightService;
        _pdfService = pdfService;
    }
    
    public async Task ExtractInsights([FromForm] IFormFile file, [FromQuery] string? model)
    {
        var text = await _pdfService.ExtractText(file);
        var request = new InsightRequest
        {
            Text = text,
            Model = model
        };
        await _insightService.ExtractInsights(HttpContext, request);
    }
    
}