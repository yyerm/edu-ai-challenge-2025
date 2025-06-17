# Service Analysis Report Generator

A lightweight console application that generates comprehensive, markdown-formatted reports about services or products using OpenAI's GPT-4.1-mini model.

## Prerequisites

- .NET 8 runtime
- OpenAI API key with access to gpt-4.1-mini model

## Installation

1. Clone or download the repository
2. Build the project using Visual Studio 2022 or .NET CLI:

dotnet build


## Usage

Run the application from the command line with:

dotnet run --project Task9.CLI <service_name_or_description>


Examples:

dotnet run --project Task9.CLI Spotify dotnet run --project Task9.CLI "A note-taking app with AI capabilities"


If you run the application without arguments, it will prompt you to enter a service name or description.

## API Key Management

The application handles your OpenAI API key in three ways (in order of priority):

1. Environment variable: Set `OPENAI_API_KEY` in your system environment variables
2. Local file: Store your key in `~/.openai/api_key.txt`
3. Manual entry: Enter your key when prompted by the application

When entering your key manually, you'll have the option to save it for future use.

## Report Content

The generated report includes the following sections:

- Brief History (founding year, milestones, founders)
- Target Audience (primary user segments)
- Core Features (key functionalities)
- Unique Selling Points (key differentiators)
- Business Model (revenue sources)
- Tech Stack Insights (technologies used)
- Perceived Strengths (advantages)
- Perceived Weaknesses (limitations)

## Output Options

After generating the report, the application offers to save it as a markdown file, which you can then convert to other formats or use in documentation.