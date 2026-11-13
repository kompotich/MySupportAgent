namespace MySupportAgent.Core.Instructions;

internal static class AgentInstructions
{
    public static string Main => ReadPrompt("MySupportAgent.Core.Instructions.MainAgent.md");
    
    public static string Resume => ReadPrompt("MySupportAgent.Core.Instructions.ResumeAgent.md");

    public static string QuickNote => ReadPrompt("MySupportAgent.Core.Instructions.QuickNoteAgent.md");

    public static string Hh => ReadPrompt("MySupportAgent.Core.Instructions.HhAgent.md");

    private static string ReadPrompt(string resourceName)
    {
        using var stream = typeof(AgentInstructions).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Встроенная инструкция агента '{resourceName}' не найдена.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
