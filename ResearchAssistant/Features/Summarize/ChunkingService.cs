namespace ResearchAssistant.Features.PdfParsing;

public class ChunkingService
{
    public List<string> SplitIntoChunks(string text, int maxWords)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();

        for (int i = 0; i < words.Length; i += maxWords)
        {
            var chunk = string.Join(" ", words.Skip(i).Take(maxWords));
            chunks.Add(chunk);
        }

        return chunks;
    }
}