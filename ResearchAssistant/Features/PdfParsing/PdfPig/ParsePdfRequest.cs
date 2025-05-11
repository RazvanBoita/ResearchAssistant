using System.ComponentModel.DataAnnotations;

namespace ResearchAssistant.Features.PdfParsing.PdfPig;

public class ParsePdfRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}