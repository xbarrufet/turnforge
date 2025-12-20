namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface IAgentSpawnStrategy
{
    IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext ctx);

}
