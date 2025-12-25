using TurnForge.Engine.Appliers.Entity.Results;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Events;

namespace TurnForge.Engine.Events; // Moving to Events namespace

public sealed record AgentSpawnedEvent : GameEvent
{
    public EntityId AgentId { get; init; }
    public string DefinitionId { get; init; }
    public Position Position { get; init; }

    public override string Description => $"Agent '{DefinitionId}' spawned at {Position}";

    public AgentSpawnedEvent(
        EntityId agentId,
        string definitionId,
        Position position,
        EventOrigin origin = EventOrigin.Command)
        : base(origin)
    {
        AgentId = agentId;
        DefinitionId = definitionId;
        Position = position;
    }
}
