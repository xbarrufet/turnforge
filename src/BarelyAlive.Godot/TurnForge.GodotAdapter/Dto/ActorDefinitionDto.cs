namespace TurnForge.GodotAdapter.Dto;

public readonly record struct ActorDefinitionDto(string actorKind, string actorId, PositionDto initialPosition)
{
    public string ActorKind { get; init; } = actorKind;
    public string ActorId { get; init; } = actorId;
    public PositionDto InitialPosition { get; init; } = initialPosition;
    
}