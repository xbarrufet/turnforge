using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components; // (Assegura't de tenir PositionComponent accessible)

namespace TurnForge.Engine.Traits.Standard;

public class PositionTrait : BaseComponentTrait<BasePositionComponent>
{
    public Position InitialPosition { get; }

    public PositionTrait(Position position)
    {
        InitialPosition = position;
    }
    
    // Constructor buit per defecte (posició desconeguda/spawner)
    public PositionTrait() : this(Position.Empty) { }
}