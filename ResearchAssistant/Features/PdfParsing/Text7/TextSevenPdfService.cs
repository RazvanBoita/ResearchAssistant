using System.Text;

namespace ResearchAssistant.Features.PdfParsing.Text7;

public class TextSevenPdfService
{
    public async Task<string> ExtractText(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var text = new StringBuilder();
        var pdfReader = new iText.Kernel.Pdf.PdfReader(memoryStream);
        var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfReader);
            
        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            var page = pdfDocument.GetPage(i);
            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
            var currentText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                
            text.AppendLine(currentText);
        }

        pdfDocument.Close();
        pdfReader.Close();
        return text.ToString();
    }
}