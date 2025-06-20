using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Task11.Interfaces;

namespace Task11.Services;

public class GptSummarizationService : ISummarizationService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public GptSummarizationService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> SummarizeAsync(string transcript)
    {
        var prompt = $"Summarize the following transcript in a concise way, focusing on main points and takeaways:\n\n{transcript}";

        var requestBody = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant that summarizes meeting transcripts." },
                new { role = "user", content = prompt }
            },
            max_tokens = 512,
            temperature = 0.5
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"GPT API error: {response.StatusCode}");
            return string.Empty;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var summary = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return summary ?? string.Empty;
    }
}