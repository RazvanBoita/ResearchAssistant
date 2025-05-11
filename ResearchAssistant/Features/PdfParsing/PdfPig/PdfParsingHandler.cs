namespace ResearchAssistant.Features.PdfParsing.PdfPig;

public class PdfParsingHandler
{
    private readonly PdfParser _pdfParser;

    public PdfParsingHandler(PdfParser pdfParser)
    {
        _pdfParser = pdfParser;
    }

    public ParsePdfResponse Handle(ParsePdfRequest request)
    {
        var text = _pdfParser.ExtractText(request.File);
        return new ParsePdfResponse { ExtractedText = text };
    }
}