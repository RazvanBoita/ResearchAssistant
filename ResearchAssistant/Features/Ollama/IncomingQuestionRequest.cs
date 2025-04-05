namespace ResearchAssistant.Features.Ollama;

public class IncomingQuestionRequest
{
    public string Question { get; set; }
    public string Model { get; set; }
}