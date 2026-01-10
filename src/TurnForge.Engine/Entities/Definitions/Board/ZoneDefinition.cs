using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Board;

public abstract class ZoneDefinition : BaseGameEntityDefinition
{

    public ZoneDefinition(
        string definitionId, Category category) : base(definitionId, category)
    {
        // ZoneTrait removed - ZoneTopology is a direct property on Zone
    }

    public ZoneDefinition(
        string definitionId) : base(definitionId, Zone.ZoneDefaultCategory)
    {
        // ZoneTrait removed - ZoneTopology is a direct property on Zone
    }



}
