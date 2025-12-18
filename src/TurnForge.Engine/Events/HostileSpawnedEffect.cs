using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands.GameStart.Effects;

public sealed record NpcSpawnedEffect(
    ActorId UnitId,
    Position Position
) : IGameEffect; 

