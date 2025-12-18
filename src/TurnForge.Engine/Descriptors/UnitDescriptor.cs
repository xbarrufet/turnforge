using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record UnitDescriptor(
    UnitTypeId TypeId,
    Position? Position,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null);
