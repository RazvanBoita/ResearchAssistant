using Microsoft.AspNetCore.Mvc;

namespace ResearchAssistant.Features.PdfParsing.Text7;

[ApiController]
[Route("api/pdf")]
public class PdfController : ControllerBase
{
    private readonly TextSevenPdfService _pdfService;

    public PdfController(TextSevenPdfService pdfService)
    {
        _pdfService = pdfService;
    }
    
    [HttpPost("parse/text7")]
    public async Task<IActionResult> ExtractText([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded or file is empty.");
        }

        if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Uploaded file is not a PDF.");
        }

        try
        {
            var text = await _pdfService.ExtractText(file);
            return Ok(new { Text = text });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}