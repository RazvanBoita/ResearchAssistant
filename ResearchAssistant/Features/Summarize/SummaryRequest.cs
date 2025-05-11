namespace ResearchAssistant.Features.Summarize;

public class SummaryRequest
{
    public string Text { get; set; }
    public string? Model { get; set; }
}