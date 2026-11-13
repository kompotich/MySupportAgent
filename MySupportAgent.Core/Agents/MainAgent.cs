using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MySupportAgent.Core.Configuration;
using MySupportAgent.Core.Instructions;
using OllamaSharp;

namespace MySupportAgent.Core.Agents;

public class MainAgent : IMySupportAgent
{
    public AIAgent CreateAgent()
    {
        var quickNoteAgent = new QuickNoteAgent().CreateAgent().AsAIFunction();
        var resumeAgent = new ResumeAgent().CreateAgent().AsAIFunction();
        var hhAgent = new HhAgent().CreateAgent().AsAIFunction();
        
        var config = Config.Load();

        return new OllamaApiClient(config.Endpoint, config.ModelName).AsAIAgent(
            name: "MainAgent",
            instructions: AgentInstructions.Main,
            tools: new List<AITool>() { quickNoteAgent, resumeAgent, hhAgent });
    }
}