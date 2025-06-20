using System.Net.Http.Headers;
using System.Text.Json;
using Task11.Interfaces;
using Task11.Models;

namespace Task11.Services;

public class WhisperTranscriptionService : ITranscriptionService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public WhisperTranscriptionService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<TranscriptionResult> TranscribeAsync(string audioFilePath)
    {
        using var form = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(audioFilePath);
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav"); // Adjust if needed

        form.Add(fileContent, "file", Path.GetFileName(audioFilePath));
        form.Add(new StringContent("whisper-1"), "model");
        form.Add(new StringContent("en"), "language");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", form);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Whisper API error: {response.StatusCode}");
            return new TranscriptionResult();
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        string transcript = doc.RootElement.GetProperty("text").GetString() ?? string.Empty;

        double duration = GetAudioDuration(audioFilePath);

        return new TranscriptionResult
        {
            Transcript = transcript,
            AudioDurationSeconds = duration
        };
    }

    private double GetAudioDuration(string filePath)
    {
    
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".wav":
                    using (var reader = new NAudio.Wave.WaveFileReader(filePath))
                    {
                        return reader.TotalTime.TotalSeconds;
                    }
                case ".mp3":
                    using (var reader = new NAudio.Wave.Mp3FileReader(filePath))
                    {
                        return reader.TotalTime.TotalSeconds;
                    }
                // Add more formats as needed
                default:
                    Console.WriteLine("Unsupported audio format for duration extraction.");
                    return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading audio duration: {ex.Message}");
            return 0;
        }
    }
}