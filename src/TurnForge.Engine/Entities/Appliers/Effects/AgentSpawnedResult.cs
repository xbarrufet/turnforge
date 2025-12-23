using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers.Results;

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
