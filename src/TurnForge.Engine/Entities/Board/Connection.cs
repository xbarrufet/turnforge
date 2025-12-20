using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

using TurnForge.Engine.Entities.Components;

public sealed class Connection : GameEntity
{
    public TileId FromAreaId { get; init; }
    public TileId ToAreaId { get; init; }
    public bool IsOpen { get; set; }

    public Connection(EntityId id, TileId fromAreaId, TileId toAreaId, BehaviourComponent behaviourComponent, bool isOpen = true) : base(id)
    {
        FromAreaId = fromAreaId;
        ToAreaId = toAreaId;
        IsOpen = isOpen;
        AddComponent(behaviourComponent);
    }
}