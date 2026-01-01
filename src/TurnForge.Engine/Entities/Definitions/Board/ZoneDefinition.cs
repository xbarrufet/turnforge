using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Board;

public  abstract class ZoneDefinition(
        string definitionId,
        string category,
        IZoneBound Bound) : BaseGameEntityDefinition(definitionId, category)
    {
        public IZoneBound ZoneBound { get; } = Bound;
    }
