using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

public sealed record PropDescriptor(
    PropTypeId TypeId,
    Position? Position = null,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null) : IGameEntityDescriptor<Prop>;
