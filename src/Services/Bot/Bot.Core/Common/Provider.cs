using System.Text.Json.Serialization;

namespace Bot.Core.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Provider : byte
{
    OpenAi,
    Anthropic,
    Google,
    GoogleVertexAi,
    Groq,
    Mistral,
    Bedrock,
    HuggingFace,
    Grok,
    OpenRouter,
    Ollama,
    Perplexity,
    GitHubModels,
    Cerebras
}