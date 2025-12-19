using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Descriptors;

public sealed record UnitDescriptor(
    UnitTypeId TypeId,
    Position? Position,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null);
