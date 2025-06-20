using System.Globalization;
using Task11.Interfaces;
using Task11.Models;

namespace Task11.Services;

public class TranscriptAnalyticsService : IAnalyticsService
{
    public TranscriptAnalytics Analyze(string transcript, double audioDurationSeconds)
    {
        var words = transcript.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        int wpm = 0;
        if (audioDurationSeconds > 0)
            wpm = (int)Math.Round(wordCount / (audioDurationSeconds / 60.0));

        var topics = ExtractTopics(transcript);

        return new TranscriptAnalytics
        {
            WordCount = wordCount,
            SpeakingSpeedWpm = wpm,
            FrequentlyMentionedTopics = topics
        };
    }

    private List<TopicMention> ExtractTopics(string transcript)
    {
        // Simple keyword frequency extraction (improve with NLP for production)
        var wordGroups = transcript
            .Split(new[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim().ToLowerInvariant())
            .Where(w => w.Length > 3) // Ignore short/common words
            .GroupBy(w => w)
            .Select(g => new { Word = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(10)
            .Select(g => new TopicMention { Topic = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Word), Mentions = g.Count })
            .ToList();

        return wordGroups;
    }
}