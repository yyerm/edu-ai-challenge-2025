namespace Task11.Interfaces;

public interface ISummarizationService
{
    Task<string> SummarizeAsync(string transcript);
}