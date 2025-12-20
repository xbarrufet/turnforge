using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

public sealed record AgentDescriptor(
    AgentTypeId TypeId,
    Position? Position,
    IReadOnlyList<IActorBehaviour>? ExtraBehaviours = null) : IGameEntityDescriptor<Agent>;
