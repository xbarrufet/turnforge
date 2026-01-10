using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Entities.Actors; // Base Actor
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public class Prop : Actor
{
    public static readonly Category PropDefaultCategory = new("PropCategory");

    // Constructor for Builder (with startPosition)
    public Prop(
        EntityId id,
        string definitionId,
        string name,
        Category category,
        IBoardPositionId startPosition)
        : base(id, definitionId, name, category, startPosition)
    {
    }

    public Prop(
        EntityId id,
        string definitionId,
        string name,
        Category category)
        : base(id, definitionId, name, category)
    {
    }

    public Prop(EntityId id,
        string definitionId
    ) : this(id, definitionId, definitionId, PropDefaultCategory)
    {

    }

    public Prop(EntityId id,
        string definitionId,
        Category category
    ) : this(id, definitionId, definitionId, category)
    {

    }
}