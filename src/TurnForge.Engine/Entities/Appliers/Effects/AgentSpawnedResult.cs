using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers.Results;

public sealed record AgentSpawnedResult(
    EntityId AgentId,
    AgentTypeId AgentType,
    Position Position
) : IGameEffect;
