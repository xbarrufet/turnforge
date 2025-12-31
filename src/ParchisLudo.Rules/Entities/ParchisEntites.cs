using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board.Definitions;
using TurnForge.Engine.Traits;
using TurnForge.Engine.Traits.Interfaces;
using TurnForge.Engine.ValueObjects;
using static Parchis.Rules.Board.ParchisBoard;

namespace Parchis.Rules.Entities;

//definim els traits i les entittaas que forme part del joc

public class ColorTrait(PlayerColor color) : BaseDataTrait
{
    public PlayerColor Color { get; init; } = color;
}

public class SafeZoneTrait(bool safe=false) : BaseDataTrait
{
    public bool Safe { get; init; } = safe;
}

public class BlockZoneTrait(bool block=false) : BaseDataTrait
{
    public bool Block { get; init; } = block;
}

//necessaria Sempre
public class ParchisPlayerDefinition : PlayerDefinition
{
    public PlayerColor Color { get; }

    public ParchisPlayerDefinition(string definitionId, PlayerId playerId, PlayerColor color) 
        : base(definitionId, playerId)
    {
        Color = color;
        AddTrait(new ColorTrait(color));
    }
}


public class ParchisPawnDefinition : AgentDefinition
{
    public PlayerColor Color { get; }

    public ParchisPawnDefinition(string definitionId, PlayerId playerId, PlayerColor color) 
        : base(definitionId, playerId)
    {
        Color = color;
        AddTrait(new ColorTrait(color));
    }
}

public class SecureCellDefinition : ZoneDefinition  {
    public SecureCellDefinition(string definitionId, TileId tileId) : 
                base(definitionId, "Cell", 
                new TileSetZoneBound(tileId)) {
        AddTrait(new SafeZoneTrait());
        AddTrait(new BlockZoneTrait());
    }
}




