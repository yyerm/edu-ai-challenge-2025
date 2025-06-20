using Task11.Models;

namespace Task11.Interfaces;

public interface IAnalyticsService
{
    TranscriptAnalytics Analyze(string transcript, double audioDurationSeconds);
}