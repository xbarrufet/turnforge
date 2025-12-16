using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor(ActorId id, Position position)
{
    public ActorId Id { get; } = id;
    public Position Position { get; protected set; } = position;
    
    private readonly HashSet<IActorTrait> _traits = new();
    
    public void AddTrait(IActorTrait trait)
        => _traits.Add(trait);

    public bool HasTrait<TTrait>()
        where TTrait : IActorTrait
        => _traits.Any(t => t is TTrait);

    public IEnumerable<IActorTrait> Traits => _traits;
}