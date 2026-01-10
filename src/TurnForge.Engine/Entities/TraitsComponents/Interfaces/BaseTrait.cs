namespace TurnForge.Engine.Entities.TraitsComponents.Interfaces;

public class BaseTrait : ITrait
{
    public BaseTrait()
    {
        IsInitialized = false;
    }

    public bool IsInitialized { get; protected set; }

    public bool StackAllowed { get; init; } = false;
}