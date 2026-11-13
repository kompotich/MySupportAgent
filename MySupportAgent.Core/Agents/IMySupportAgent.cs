using Microsoft.Agents.AI;

namespace MySupportAgent.Core.Agents;

public interface IMySupportAgent
{
    AIAgent CreateAgent();
}
