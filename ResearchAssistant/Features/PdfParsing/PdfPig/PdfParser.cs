using UglyToad.PdfPig;

namespace ResearchAssistant.Features.PdfParsing.PdfPig;

public class PdfParser
{
    public string ExtractText(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var document = PdfDocument.Open(stream);
        
        var text = string.Join("\n", document.GetPages().Select(p => p.Text));
        return text;
    }
}