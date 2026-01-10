using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Definitions.Player;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Traits;
using TurnForge.Engine.ValueObjects;
using static ParchisLudo.Rules.Board.ParchisBoard;

namespace ParchisLudo.Rules.Entities;

//definim els traits i les entittaas que forme part del joc
/*
public class ColorTrait(PlayerColor color) : BaseTrait
{
    public PlayerColor Color { get; init; } = color;
}

public class SpawnZoneTrait() : BaseTrait
{
}

public class SafeZoneTrait(bool safe = false) : BaseTrait
{
    public bool Safe { get; init; } = safe;
}

public class BlockZoneTrait(bool block = false) : BaseTrait
{
    public bool Block { get; init; } = block;
}

public class CenterTrait() : BaseTrait
{
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

public class PawnDefinition : AgentDefinition
{
    public PawnDefinition(string id) : base(id, id, PlayerId.From("System"))
    {
        AddTrait(new ColorTrait(PlayerColor.UNDEFINED));
    }
    public static string DefId => "pawn";
}

public class FinishLinitConnectionDefinition : ConnectionDefinition
{
    public FinishLinitConnectionDefinition(string id) : base(id,"Connections") { }
    public static string DefId => "finish_line_connection";
}

public class SpawZoneDefinition : ZoneDefinition
{
    public SpawZoneDefinition(string id) : base(id, "spawn_zone")
    {
        AddTrait(new ColorTrait(PlayerColor.UNDEFINED));
        AddTrait(new SpawnZoneTrait());
    }
    public static string DefId => "spawn_zone";
}

public class CenterZoneDefinition : ZoneDefinition
{
    public CenterZoneDefinition(string id) : base(id, "center_zone")
    {
        AddTrait(new CenterTrait());
    }
    public static string DefId => "center_zone";
}

public class SafetyZoneDefinition : ZoneDefinition
{
    public SafetyZoneDefinition(string id) : base(id, "safety_zone")
    {
        AddTrait(new SafeZoneTrait(true));
    }
    public static string DefId => "safety_zone";
}

*/
