using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Traits.Standard;

public class PositionTrait : BaseComponentTrait<BasePositionComponent>
{
    public IBoardPosition InitialPosition { get; }

    public PositionTrait(IBoardPosition position)
    {
        InitialPosition = position;
    }
    
    // Constructor buit per defecte (posició desconeguda/spawner)
    public PositionTrait() : this(TilePosition.Empty) { }
}