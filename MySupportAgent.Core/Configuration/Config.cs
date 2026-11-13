using System.Text.Json;

namespace MySupportAgent.Core.Configuration;

internal sealed class Config
{
    public OllamaSettings? Ollama { get; init; }

    public ResumeSettings? Resume { get; init; }

    public HhUrlsSettings? HhUrls { get; init; }

    public static (Uri Endpoint, string ModelName, string ResumeDirectory, HhUrlsSettings HhUrls) Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "config.json");
        var settings = JsonSerializer.Deserialize<Config>(File.ReadAllText(path))
            ?? throw new InvalidDataException($"Файл конфигурации '{path}' пуст или содержит некорректные данные.");

        var endpoint = settings.Ollama?.OllamaEndpoint;
        if (endpoint is null || !endpoint.IsAbsoluteUri)
        {
            throw new InvalidDataException($"Файл конфигурации '{path}' должен содержать абсолютный URI в параметре Ollama:OllamaEndpoint.");
        }

        var modelName = settings.Ollama?.ModelName;
        if (string.IsNullOrWhiteSpace(modelName))
        {
            throw new InvalidDataException($"Файл конфигурации '{path}' должен содержать имя модели в параметре Ollama:ModelName.");
        }

        var resumeDirectory = settings.Resume?.ResumeDirectory;
        if (string.IsNullOrWhiteSpace(resumeDirectory))
        {
            throw new InvalidDataException($"Файл конфигурации '{path}' должен содержать каталог резюме в параметре Resume:ResumeDirectory.");
        }

        var hhUrls = settings.HhUrls
            ?? throw new InvalidDataException($"Файл конфигурации '{path}' должен содержать секцию HhUrls.");
        ValidateUrl(path, nameof(HhUrlsSettings.CSharpDeveloper), hhUrls.CSharpDeveloper);
        ValidateUrl(path, nameof(HhUrlsSettings.SystemAnalyst), hhUrls.SystemAnalyst);
        ValidateUrl(path, nameof(HhUrlsSettings.SqlDeveloper), hhUrls.SqlDeveloper);
        ValidateUrl(path, nameof(HhUrlsSettings.QAAutomationEngineer), hhUrls.QAAutomationEngineer);
        ValidateUrl(path, nameof(HhUrlsSettings.SupportEngineer), hhUrls.SupportEngineer);

        return (endpoint, modelName, resumeDirectory, hhUrls);
    }

    private static void ValidateUrl(string path, string propertyName, string? value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            throw new InvalidDataException($"Файл конфигурации '{path}' должен содержать абсолютный URI в параметре HhUrls:{propertyName}.");
        }
    }
}

internal sealed class OllamaSettings
{
    public Uri? OllamaEndpoint { get; init; }

    public string? ModelName { get; init; }
}

internal sealed class ResumeSettings
{
    public string? ResumeDirectory { get; init; }
}

internal sealed class HhUrlsSettings
{
    public string? CSharpDeveloper { get; init; }

    public string? SystemAnalyst { get; init; }

    public string? SqlDeveloper { get; init; }

    public string? QAAutomationEngineer { get; init; }

    public string? SupportEngineer { get; init; }
}
