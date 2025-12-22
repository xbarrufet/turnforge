namespace BarelyAlive.Rules.Apis.Messaging;

public sealed record SurvivorDefinition(
    string Id,
    string Name,
    string Description,
    int MaxHealth,
    int MaxMovement
);
