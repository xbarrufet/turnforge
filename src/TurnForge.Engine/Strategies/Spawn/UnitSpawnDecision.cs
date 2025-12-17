using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn;

public sealed record UnitSpawnDecision
    (UnitDescriptor Descriptor,
        Position Position)
{
}