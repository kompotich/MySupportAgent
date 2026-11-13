using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MySupportAgent.Core.Configuration;
using MySupportAgent.Core.Instructions;
using MySupportAgent.Core.Tools;
using OllamaSharp;

namespace MySupportAgent.Core.Agents;

public sealed class ResumeAgent : IMySupportAgent
{
    private static IList<AITool> GetTools()
    {
        return
        [
            AIFunctionFactory.Create(ResumeTools.ListResumeFiles),
            AIFunctionFactory.Create(ResumeTools.GetResumeText),
            AIFunctionFactory.Create(ResumeTools.SearchResumeByQuery),
        ];
    }

    public AIAgent CreateAgent()
    {
        var config = Config.Load();

        return new OllamaApiClient(config.Endpoint, config.ModelName).AsAIAgent(
            name: "ResumeAgent",
            instructions: AgentInstructions.Resume,
            tools: GetTools()
        );
    }
}