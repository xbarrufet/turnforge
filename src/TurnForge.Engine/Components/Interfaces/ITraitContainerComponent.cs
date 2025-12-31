using TurnForge.Engine.Traits;
using TurnForge.Engine.Traits.Interfaces;

namespace TurnForge.Engine.Components.Interfaces;

/// <summary>
/// Component that manages traits attached to an entity.
/// Traits define dynamic logic and rules that modify entity behavior at runtime.
/// </summary>
public interface ITraitContainerComponent : IGameEntityComponent
{

    IReadOnlyList<BaseDataTrait> Traits {get;}
    // Query methods
    bool HasTrait<T>() where T : IDataTrait;
    T? GetTrait<T>() where T : IDataTrait;
    T GetRequiredTrait<T>() where T : IDataTrait;
    bool TryGetTrait<T>(out T? trait) where T : IDataTrait;
    


    // Mutation methods for runtime trait management
    void AddTrait(IDataTrait trait);
    bool RemoveTrait<T>() where T : IDataTrait;

    public static ITraitContainerComponent Empty()
    {
        return new TraitContainerComponent();
    }
}
