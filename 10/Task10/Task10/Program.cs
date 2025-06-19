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
            Console.WriteLine("Please provide your filtering request:");
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

        // Load products from the JSON file
        var productsJson = File.ReadAllText("products.json");
        var products = JsonSerializer.Deserialize<List<Product>>(productsJson);

        if (products == null || !products.Any())
        {
            Console.WriteLine("Error: Failed to load products.");
            return;
        }

        Console.WriteLine($"Loaded {products.Count} products.");
        Console.WriteLine($"Query: {input}");

        // Set up the API call to OpenAI
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Create the function definition for filtering products
        var filterFunction = new
        {
            name = "filter_products",
            description = "Filter products based on user preferences and return matching products",
            parameters = new
            {
                type = "object",
                properties = new
                {
                    matchingProducts = new
                    {
                        type = "array",
                        description = "List of products that match the user's preferences",
                        items = new
                        {
                            type = "object",
                            properties = new
                            {
                                name = new { type = "string" },
                                category = new { type = "string" },
                                price = new { type = "number" },
                                rating = new { type = "number" },
                                in_stock = new { type = "boolean" }
                            }
                        }
                    }
                },
                required = new[] { "matchingProducts" }
            }
        };

        // Create the API request
        var requestBody = new
        {
            model = "gpt-4.1-mini",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a helpful shopping assistant. Filter products based on user preferences."
                },
                new
                {
                    role = "user",
                    content = $"Here is a list of products: {productsJson}\n\nFilter these products based on the following request: {input}"
                }
            },
            functions = new[] { filterFunction },
            function_call = new { name = "filter_products" }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        // Make the API call
        Console.WriteLine("Calling OpenAI API to process your query...");
        var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: API call failed with status code {response.StatusCode}");
            Console.WriteLine($"Response: {responseJson}");
            return;
        }

        // Parse the API response
        var jsonResponse = JsonDocument.Parse(responseJson);
        var functionCall = jsonResponse.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("function_call");

        var functionName = functionCall.GetProperty("name").GetString();
        var functionArgs = functionCall.GetProperty("arguments").GetString();

        if (functionName != "filter_products" || string.IsNullOrEmpty(functionArgs))
        {
            Console.WriteLine("Error: Unexpected API response format.");
            return;
        }

        // Parse the function arguments to get the filtered products directly from OpenAI
        var filterResult = JsonSerializer.Deserialize<FilterResult>(functionArgs);
        var finalProducts = filterResult?.matchingProducts ?? new List<Product>();

        // Display the results
        Console.WriteLine("\nFiltered Products:");
        if (finalProducts.Any())
        {
            var index = 1;
            foreach (var product in finalProducts)
                Console.WriteLine(
                    $"{index++}. {product.name} - ${product.price}, Rating: {product.rating}, {(product.in_stock ? "In Stock" : "Out of Stock")}");
        }
        else
        {
            Console.WriteLine("No products match your criteria.");
        }

        // Ask if the user wants to save the results
        Console.Write("\nDo you want to save this search to a file? (y/n): ");
        var saveResponse = Console.ReadLine()?.Trim().ToLower();

        if (saveResponse == "y" || saveResponse == "yes")
        {
            // Create markdown content
            var markdownContent = new StringBuilder();
            markdownContent.AppendLine("# Product Search");
            markdownContent.AppendLine();
            markdownContent.AppendLine("## Query");
            markdownContent.AppendLine($"`{input}`");
            markdownContent.AppendLine();

            markdownContent.AppendLine("## Results");
            markdownContent.AppendLine();

            if (finalProducts.Any())
            {
                markdownContent.AppendLine("| Product | Price | Rating | Availability |");
                markdownContent.AppendLine("|--------|-------|--------|--------------|");

                foreach (var product in finalProducts)
                    markdownContent.AppendLine(
                        $"| {product.name} | ${product.price} | {product.rating} | {(product.in_stock ? "In Stock" : "Out of Stock")} |");
            }
            else
            {
                markdownContent.AppendLine("No products match the specified criteria.");
            }

            // Save to file
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"product_search_{timestamp}.md";

            File.WriteAllText(filename, markdownContent.ToString());
            Console.WriteLine($"Search results saved to {filename}");
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
}

public class Product
{
    public string name { get; set; } = string.Empty;
    public string category { get; set; } = string.Empty;
    public double price { get; set; }
    public double rating { get; set; }
    public bool in_stock { get; set; }
}

public class FilterResult
{
    public List<Product> matchingProducts { get; set; } = new List<Product>();
}