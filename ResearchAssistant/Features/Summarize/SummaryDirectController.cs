using Microsoft.AspNetCore.Mvc;
using ResearchAssistant.Features.PdfParsing.Text7;

namespace ResearchAssistant.Features.Summarize;

[ApiController]
[Route("api/summary")]
public class SummaryDirectController : ControllerBase
{
    private readonly SummaryService _summaryService;
    private readonly TextSevenPdfService _pdfService;

    public SummaryDirectController(SummaryService summaryService, TextSevenPdfService pdfService)
    {
        _summaryService = summaryService;
        _pdfService = pdfService;
    }
    
    public async Task Summarize([FromForm] IFormFile file, [FromQuery] string? model)
    {
        var text = await _pdfService.ExtractText(file);
        var request = new SummaryRequest
        {
            Text = text,
            Model = model
        };
        await _summaryService.Summarize(HttpContext, request);
    }
    
}