namespace Task11.Models;

public class TranscriptAnalytics
{
    public int WordCount { get; set; }
    public int SpeakingSpeedWpm { get; set; }
    public List<TopicMention> FrequentlyMentionedTopics { get; set; } = new();
}