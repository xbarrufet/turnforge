using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Definitions;

public sealed record PropDescriptor(
    ActorId Id,
    string CustomType,
    IReadOnlyDictionary<string, IReadOnlyList<ActorTraitDefinition>> Traits,
    Position? Position=null);
