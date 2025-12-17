using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record UnitDescriptor(
    ActorId Id,
    int Health,
    int ActionPoints,
    string CustomType,
    IReadOnlyDictionary<string, IReadOnlyList<ActorTraitDefinition>> Traits);
