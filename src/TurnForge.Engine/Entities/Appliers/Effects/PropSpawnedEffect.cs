using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.GameStart.Effects;

public sealed record PropSpawnedEffect(
    EntityId PropId,
    PropTypeId PropType,
    Position Position
) : IGameEffect;
