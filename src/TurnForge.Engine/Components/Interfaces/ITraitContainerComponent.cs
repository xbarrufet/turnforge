using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Components.Interfaces;

/// <summary>
/// Component that manages traits attached to an entity.
/// Traits define dynamic logic and rules that modify entity behavior at runtime.
/// </summary>
public interface ITraitContainerComponent : IGameEntityComponent
{
    // Query methods
    bool HasTrait<T>() where T : IBaseTrait;
    T? GetTrait<T>() where T : IBaseTrait;
    T GetRequiredTrait<T>() where T : IBaseTrait;
    bool TryGetTrait<T>(out T? trait) where T : IBaseTrait;
    
    // Mutation methods for runtime trait management
    void AddTrait(IBaseTrait trait);
    bool RemoveTrait<T>() where T : IBaseTrait;

    public static ITraitContainerComponent Empty()
    {
        return new TraitContainerComponent();
    }
}
