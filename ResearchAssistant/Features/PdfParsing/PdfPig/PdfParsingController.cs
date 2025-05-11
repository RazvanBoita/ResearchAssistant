using Microsoft.AspNetCore.Mvc;

namespace ResearchAssistant.Features.PdfParsing.PdfPig;

[ApiController]
[Route("api/pdf")]
public class PdfParsingController : ControllerBase
{
    private readonly PdfParsingHandler _parsingHandler;

    public PdfParsingController(PdfParsingHandler parsingHandler)
    {
        _parsingHandler = parsingHandler;
    }
    
    
    [HttpPost("parse/pig")]
    [ProducesResponseType(typeof(ParsePdfResponse), StatusCodes.Status200OK)]
    public ActionResult<ParsePdfResponse> ParsePdf([FromForm] IFormFile file)
    {
        var request = new ParsePdfRequest {File = file};
        var response = _parsingHandler.Handle(request);
        return Ok(response);
    }
}