namespace TurnForge.Engine.Entities.Actors.Definitions;

public abstract record AgentDefinition : ActorDefinition
{
    public int MaxHealth { get; init; }
    public int MaxActionPoints { get; init; }
    public int MaxBaseMovement  { get; init; }
}