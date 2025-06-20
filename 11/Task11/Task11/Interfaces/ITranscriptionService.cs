using Task11.Models;

namespace Task11.Interfaces;

public interface ITranscriptionService
{
    Task<TranscriptionResult> TranscribeAsync(string audioFilePath);
}