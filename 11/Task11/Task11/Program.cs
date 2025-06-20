using System.Globalization;
using System.Text.Json;
using Task11.Services;

namespace Task11;

public class Program
{
    public static async Task Main(string[] args)
    {
        string input;

        if (args.Length == 0)
        {
            Console.WriteLine("Please provide file path.");
            input = Console.ReadLine()?.Trim() ?? string.Empty;
        }
        else
        {
            input = string.Join(" ", args);
        }

        var apiKey = GetApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: API key not found. Please set your OpenAI API key.");
            return;
        }

        if (!File.Exists(input))
        {
            Console.WriteLine("Audio file not found.");
            return;
        }

        var whisperService = new WhisperTranscriptionService(apiKey);
        var gptService = new GptSummarizationService(apiKey);
        var analyticsService = new TranscriptAnalyticsService();

        Console.WriteLine("Transcribing audio...");
        var transcriptResult = await whisperService.TranscribeAsync(input);

        if (string.IsNullOrWhiteSpace(transcriptResult.Transcript))
        {
            Console.WriteLine("Transcription failed or returned empty.");
            return;
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        var baseName = Path.GetFileNameWithoutExtension(input);

        var transcriptionFile = $"{baseName}_transcription_{timestamp}.md";
        var summaryFile = $"{baseName}_summary_{timestamp}.md";
        var analysisFile = $"{baseName}_analysis_{timestamp}.json";

        await File.WriteAllTextAsync(transcriptionFile, transcriptResult.Transcript);

        Console.WriteLine("Summarizing transcript...");
        var summary = await gptService.SummarizeAsync(transcriptResult.Transcript);
        await File.WriteAllTextAsync(summaryFile, summary);

        Console.WriteLine("Analyzing transcript...");
        var analytics = analyticsService.Analyze(transcriptResult.Transcript, transcriptResult.AudioDurationSeconds);
        var analyticsJson = JsonSerializer.Serialize(analytics, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(analysisFile, analyticsJson);

        Console.WriteLine("\n--- SUMMARY ---\n");
        Console.WriteLine(summary);
        Console.WriteLine("\n--- ANALYTICS ---\n");
        Console.WriteLine(analyticsJson);
        Console.WriteLine($"\nFiles saved:\n{transcriptionFile}\n{summaryFile}\n{analysisFile}");
    }

    private static string GetApiKey()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            var keyFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".openai",
                "api_key.txt");

            if (File.Exists(keyFilePath)) apiKey = File.ReadAllText(keyFilePath).Trim();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Write("Enter your OpenAI API key: ");
            apiKey = Console.ReadLine()?.Trim();

            Console.Write("Save API key for future use? (y/n): ");
            var saveResponse = Console.ReadLine()?.Trim().ToLower();

            if (saveResponse == "y" || saveResponse == "yes")
                try
                {
                    var keyDirPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".openai");

                    Directory.CreateDirectory(keyDirPath);
                    File.WriteAllText(Path.Combine(keyDirPath, "api_key.txt"), apiKey);
                    Console.WriteLine("API key saved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save API key: {ex.Message}");
                }
        }

        return apiKey;
    }
}