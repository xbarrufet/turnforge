using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Components.Interfaces;

public sealed class Connection : GameEntity
{
    public TileId FromAreaId { get; init; }
    public TileId ToAreaId { get; init; }
    public bool IsOpen { get; set; }

    public Connection(EntityId id, TileId fromAreaId, TileId toAreaId, IBehaviourComponent behaviourComponent, bool isOpen = true) : base(id, string.Empty, string.Empty, string.Empty)
    {
        FromAreaId = fromAreaId;
        ToAreaId = toAreaId;
        IsOpen = isOpen;
        AddComponent(behaviourComponent);
    }
}