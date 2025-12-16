using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Connection(ConnectionId id, AreaId fromAreaId, AreaId toAreaId, bool isOpen= true)
{
    public ConnectionId Id { get; init; } = id;
    public AreaId FromAreaId { get; init; } = fromAreaId;
    public AreaId ToAreaId { get; init; } = toAreaId;
    public bool IsOpen { get; set; } = isOpen;

    private readonly HashSet<IConnectionTrait> _traits = [];
    
    public void AddTrait(IConnectionTrait trait)
        => _traits.Add(trait);

    public bool HasTrait<T>() where T : IConnectionTrait
        => _traits.Any(t => t is T);

    public T? GetTrait<T>() where T : class, IConnectionTrait
        => _traits.OfType<T>().FirstOrDefault();
}