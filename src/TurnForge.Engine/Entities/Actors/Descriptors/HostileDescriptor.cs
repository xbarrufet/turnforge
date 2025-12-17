using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record HostileDescriptor(
    ActorId Id,
    int Health,
    string CustomType,
    IReadOnlyDictionary<string, IReadOnlyList<ActorTraitDefinition>> Traits);
