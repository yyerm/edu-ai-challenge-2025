# Product Search Application

A console application that lets you search for products using natural language queries. The application leverages OpenAI's function calling API to understand your preferences and filter products accordingly.

## Setup

### Prerequisites
- .NET 8.0 SDK or later
- An OpenAI API key

### API Key Configuration
You can provide your OpenAI API key in one of three ways:
1. Environment variable: Set the `OPENAI_API_KEY` environment variable
2. Store in a file: Create a file at `~/.openai/api_key.txt`
3. Input when prompted: The application will ask for your API key if not found

## Usage

Run the application using the dotnet CLI:

dotnet run


Or with arguments:

dotnet run "I need electronics under $200 with at least 4.5 rating that are in stock"



### How It Works

1. Enter your product search query in natural language
2. The application sends your query to OpenAI's API
3. Search parameters are extracted and applied to filter the product database
4. Matching products are displayed with their details
5. You can save results to a Markdown file for future reference

### Example Queries

- "Show me kitchen appliances under $100"
- "I need fitness equipment with at least 4.5 star rating"
- "Find me electronics that are in stock"
- "Books about programming with good ratings"
- "Clothing items under $30 that are available now"

## Output

The application displays filtered products in the console with:
- Product name
- Price
- Rating
- Stock availability

### Saving Results

When prompted, type `y` or `yes` to save your search results to a Markdown file. The file will be saved in the current directory with the naming format `product_search_YYYYMMDD_HHMMSS.md`.

## Data Source

The application uses a local `products.json` file as its product database. Make sure this file is present in the same directory as the executable.

