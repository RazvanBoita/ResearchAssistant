using System.Text;
using Microsoft.AspNetCore.Mvc;
using ResearchAssistant.Features.Summarize;

namespace ResearchAssistant.Features.PdfParsing;

[ApiController]
[Route("api/pdf")]
public class PdfParsingController : ControllerBase
{
    private readonly PdfParsingHandler _parsingHandler;

    public PdfParsingController(PdfParsingHandler parsingHandler)
    {
        _parsingHandler = parsingHandler;
    }
    
    
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ParsePdfResponse), StatusCodes.Status200OK)]
    public ActionResult<ParsePdfResponse> ParsePdf([FromForm] IFormFile file)
    {
        var request = new ParsePdfRequest {File = file};
        var response = _parsingHandler.Handle(request);
        return Ok(response);
    }
}