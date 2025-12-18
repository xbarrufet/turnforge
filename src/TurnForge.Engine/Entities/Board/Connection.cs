using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class Connection(ConnectionId id, AreaId fromAreaId, AreaId toAreaId, bool isOpen= true)
{
    public ConnectionId Id { get; init; } = id;
    public AreaId FromAreaId { get; init; } = fromAreaId;
    public AreaId ToAreaId { get; init; } = toAreaId;
    public bool IsOpen { get; set; } = isOpen;

    private readonly HashSet<IConnectionBehaviour> _Behaviours = [];
    
    public void AddBehaviour(IConnectionBehaviour Behaviour)
        => _Behaviours.Add(Behaviour);

    public bool HasBehaviour<T>() where T : IConnectionBehaviour
        => _Behaviours.Any(t => t is T);

    public T? GetBehaviour<T>() where T : class, IConnectionBehaviour
        => _Behaviours.OfType<T>().FirstOrDefault();
}