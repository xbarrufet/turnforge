using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record PropDescriptor(
    PropTypeId TypeId,
    Position? Position = null,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null);
