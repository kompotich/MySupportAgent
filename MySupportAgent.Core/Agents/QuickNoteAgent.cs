using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MySupportAgent.Core.Configuration;
using MySupportAgent.Core.Instructions;
using MySupportAgent.Core.Tools;
using OllamaSharp;

namespace MySupportAgent.Core.Agents;

public sealed class QuickNoteAgent : IMySupportAgent
{
    private static IList<AITool> GetTools()
    {
        return
        [
            AIFunctionFactory.Create(QuickNoteTools.GetAllQuickNotes),
            AIFunctionFactory.Create(QuickNoteTools.GetQuickNoteById),
            AIFunctionFactory.Create(QuickNoteTools.GetQuickNoteByTitle),
            AIFunctionFactory.Create(QuickNoteTools.CreateQuickNote),
            AIFunctionFactory.Create(QuickNoteTools.DeleteAllQuickNotes),
            AIFunctionFactory.Create(QuickNoteTools.DeleteQuickNoteById),
            AIFunctionFactory.Create(QuickNoteTools.DeleteQuickNoteByTitle),
        ];
    }

    public AIAgent CreateAgent()
    {
        var settings = Config.Load();
        return new OllamaApiClient(settings.Endpoint, settings.ModelName).AsAIAgent(
            name: "QuickNoteAgent",
            instructions: AgentInstructions.QuickNote,
            tools: GetTools()
        );
    }
}