using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        string input;

        // Check if the correct number of arguments is provided
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide either a service name or a description.");
            input = Console.ReadLine()?.Trim() ?? string.Empty;
        }
        else
        {
            // Combine all arguments into a single input
            input = string.Join(" ", args);
        }

        // Get the OpenAI API key securely
        var apiKey = GetApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("Error: API key not found. Please set your OpenAI API key.");
            return;
        }


        try
        {
            // Generate the report
            Console.WriteLine("Generating report... This may take a moment.");
            var report = await GenerateReport(input, apiKey);

            // Output the report
            Console.WriteLine("\n=== GENERATED REPORT ===\n");
            Console.WriteLine(report);

            // Optionally save to a file
            await SaveReportToFile(report, input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string GetApiKey()
    {
        // Try to get from environment variable
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        // If not found in env var, look for a file
        if (string.IsNullOrEmpty(apiKey))
        {
            var keyFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".openai",
                "api_key.txt");

            if (File.Exists(keyFilePath)) apiKey = File.ReadAllText(keyFilePath).Trim();
        }

        // If still not found, prompt the user
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.Write("Enter your OpenAI API key: ");
            apiKey = Console.ReadLine()?.Trim();

            // Ask if they want to save it
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

    private static async Task<string> GenerateReport(string input, string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestData = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content =
                        "You are a comprehensive business and product analyst. Generate a detailed markdown report with these specific sections:\n\n" +
                        "# [Service/Product Name] Analysis\n\n" +
                        "## Brief History\n[Founding year, key milestones, founders]\n\n" +
                        "## Target Audience\n[Primary user segments and demographics]\n\n" +
                        "## Core Features\n[Top 2-4 key functionalities]\n\n" +
                        "## Unique Selling Points\n[Key differentiators from competitors]\n\n" +
                        "## Business Model\n[How the service/product generates revenue]\n\n" +
                        "## Tech Stack Insights\n[Technologies used or likely used]\n\n" +
                        "## Perceived Strengths\n[Notable advantages and positives]\n\n" +
                        "## Perceived Weaknesses\n[Limitations, drawbacks, or areas for improvement]\n\n" +
                        "Use markdown formatting including headers, bullet points, and emphasis where appropriate. " +
                        "If specific information isn't available, provide reasonable estimates based on industry knowledge " +
                        "and mark these clearly as estimates."
                },
                new
                {
                    role = "user",
                    content = $"Generate a comprehensive analysis report for: {input}"
                }
            },
            temperature = 0.5,
            max_tokens = 2048
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"API request failed: {response.StatusCode}, {errorMessage}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        var generatedText = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";

        return generatedText;
    }

    private static async Task SaveReportToFile(string report, string input)
    {
        Console.Write("\nSave the report to a file? (y/n): ");
        var saveResponse = Console.ReadLine()?.Trim().ToLower();

        if (saveResponse == "y" || saveResponse == "yes")
        {
            // Create a valid filename from the input
            var fileName = new string(input.Take(30).ToArray()) // Take first 30 chars
                .Replace(" ", "_")
                .Replace("?", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(":", "")
                .Replace("*", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");

            fileName = $"{fileName}_report_{DateTime.Now:yyyyMMdd_HHmmss}.md";

            await File.WriteAllTextAsync(fileName, report);
            Console.WriteLine($"Report saved to {Path.GetFullPath(fileName)}");
        }
    }
}