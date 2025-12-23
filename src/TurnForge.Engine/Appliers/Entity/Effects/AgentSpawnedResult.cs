using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Appliers.Entity.Results;

public record AgentSpawnedResult : GameEffect
{
    public EntityId AgentId { get; init; }
    public string DefinitionId { get; init; }
    public Position Position { get; init; }

    public override string Description => $"Agent '{DefinitionId}' spawned at {Position}";

    public AgentSpawnedResult(
        EntityId agentId,
        string definitionId,
        Position position,
        EffectOrigin origin = EffectOrigin.Command)
        : base(origin)
    {
        AgentId = agentId;
        DefinitionId = definitionId;
        Position = position;
    }
}
