using System.ComponentModel.DataAnnotations;

namespace ResearchAssistant.Features.PdfParsing;

public class ParsePdfRequest
{
    [Required]
    public IFormFile File { get; set; } = null!;
}