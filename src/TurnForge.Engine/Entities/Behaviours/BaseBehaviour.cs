namespace TurnForge.Engine.Entities.Components;

public abstract class BaseBehaviour 
{
    public GameEntity Owner { get; internal set; } = null!;
}
