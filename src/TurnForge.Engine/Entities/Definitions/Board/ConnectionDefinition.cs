using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Board;

public abstract class ConnectionDefinition : BaseGameEntityDefinition
{
    public ConnectionDefinition(
        string definitionId,
        Category category
        ) : base(definitionId, category)
    {
        // ConnectionTrait removed - ConnectionPosition is a direct property on Connection
    }

    public ConnectionDefinition(
        string definitionId
    ) : base(definitionId, Connection.ConnectionCategory)
    {
        // ConnectionTrait removed - ConnectionPosition is a direct property on Connection
    }



}