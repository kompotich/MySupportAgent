using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MySupportAgent.Core.Configuration;
using MySupportAgent.Core.Instructions;
using MySupportAgent.Core.Tools;
using OllamaSharp;

namespace MySupportAgent.Core.Agents;

public class HhAgent : IMySupportAgent
{
    private static IList<AITool> GetTools()
    {
        return
        [
            AIFunctionFactory.Create(HhTools.GetVacancies),
        ];
    }

    public AIAgent CreateAgent()
    {
        var config = Config.Load();

        return new OllamaApiClient(config.Endpoint, config.ModelName).AsAIAgent(
            name: "HhAgent",
            instructions: AgentInstructions.Hh,
            tools: GetTools());
    }
}