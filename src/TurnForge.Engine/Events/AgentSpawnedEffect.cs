using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Events;

public sealed record AgentSpawnedEffect(
    EntityId AgentId,
    AgentTypeId AgentType,
    Position Position
) : IGameEffect;
