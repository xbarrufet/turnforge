using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers.Results;

public record AgentSpawnedResult(
    EntityId AgentId,
    string DefinitionId,
    Position Position
    ) : IGameEffect;
