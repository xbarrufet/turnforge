using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Components;

public class BehaviourComponent : IGameEntityComponent
{
    private readonly List<BaseBehaviour> _behaviours;
    public IReadOnlyList<BaseBehaviour> Behaviours => _behaviours;

    public BehaviourComponent(IEnumerable<BaseBehaviour> behaviours)
    {
        _behaviours = behaviours.ToList();
    }

    public T? GetBehaviour<T>() where T : BaseBehaviour
    {
        return _behaviours.OfType<T>().FirstOrDefault();
    }

    public bool HasBehaviour<T>() where T : BaseBehaviour
    {
        return _behaviours.Any(b => b is T);
    }

    public bool TryGetBehaviour<T>(out T? behaviour) where T : BaseBehaviour
    {
        behaviour = GetBehaviour<T>();
        return behaviour != null;
    }
    
    // Internal helper to set owner when attached
    internal void SetOwner(GameEntity owner)
    {
        foreach (var behaviour in _behaviours)
        {
            behaviour.Owner = owner;
        }
    }
}
