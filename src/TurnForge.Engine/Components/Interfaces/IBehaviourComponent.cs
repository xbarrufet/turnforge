using TurnForge.Engine.Behaviours.Interfaces;

namespace TurnForge.Engine.Components.Interfaces;

/// <summary>
/// Component that manages behaviours attached to an entity.
/// Behaviours define dynamic logic and rules that modify entity behavior at runtime.
/// </summary>
public interface IBehaviourComponent : IGameEntityComponent
{
    // Query methods
    bool HasBehaviour<T>() where T : IBaseBehaviour;
    T? GetBehaviour<T>() where T : IBaseBehaviour;
    T GetRequiredBehaviour<T>() where T : IBaseBehaviour;
    bool TryGetBehaviour<T>(out T? behaviour) where T : IBaseBehaviour;
    
    // Mutation methods for runtime behaviour management
    void AddBehaviour(IBaseBehaviour behaviour);
    bool RemoveBehaviour<T>() where T : IBaseBehaviour;

    public static IBehaviourComponent Empty()
    {
        return new BaseBehaviourComponent();
    }
}
