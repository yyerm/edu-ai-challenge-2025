# Audio Transcription, Summarization & Analytics Console App

## Overview

This application is a C#/.NET 8 console tool that:
- Accepts a spoken audio file (WAV or MP3).
- Transcribes it using OpenAI's Whisper API.
- Summarizes the transcript using OpenAI GPT (gpt-4.1-mini).
- Extracts analytics: total word count, speaking speed (words per minute), and frequently mentioned topics.
- Saves the transcription, summary, and analytics as separate timestamped files.
- Displays the summary and analytics in the console.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [OpenAI API key](https://platform.openai.com/account/api-keys)
- [NAudio](https://www.nuget.org/packages/NAudio) (installed automatically via NuGet)
- Internet connection (for API calls)

---

## Setup

1. **Clone the repository** (or copy the source files).

2. **Install dependencies**  
   In the project directory, run:
   
   dotnet restore
   
   
   
3. **Set your OpenAI API key**  
- Option 1: Set the `OPENAI_API_KEY` environment variable.
- Option 2: On first run, enter your API key when prompted. You can choose to save it for future use.

---

## Usage

1. **Build the application:**

dotnet build



2. **Run the application:**


dotnet run -- <path-to-audio-file>


- Example:
  ```sh
  dotnet run -- "meeting_audio.wav"
  ```

If you do not provide a file path as an argument, the app will prompt you to enter it.

3. **Output:**
- The app will:
  - Transcribe the audio.
  - Summarize the transcript.
  - Analyze the transcript.
  - Save three files in the working directory:
    - `<audiofile>_transcription_<timestamp>.md`
    - `<audiofile>_summary_<timestamp>.md`
    - `<audiofile>_analysis_<timestamp>.json`
  - Display the summary and analytics in the console.

---

## Supported Audio Formats

- WAV
- MP3

(You can extend support for more formats by updating the `WhisperTranscriptionService`.)

---

## How It Works

1. **Transcription:**  
The app uploads your audio file to OpenAI Whisper (`whisper-1` model) and retrieves the transcript.

2. **Summarization:**  
The transcript is sent to OpenAI GPT (`gpt-4.1-mini`) for summarization.

3. **Analytics:** 
The app calculates:
- Total word count
- Speaking speed (words per minute, based on audio duration)
- Frequently mentioned topics (by word frequency)

4. **Output:**  
Results are saved as timestamped files and shown in the console.

---

## Troubleshooting

- Ensure your audio file exists and is in a supported format.
- Ensure your OpenAI API key is valid and has sufficient quota.
- For audio duration errors, ensure the file is not corrupted and NAudio supports the format.

---

## License

This project is for educational and demonstration purposes.