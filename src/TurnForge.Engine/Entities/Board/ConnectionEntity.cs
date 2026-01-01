using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

/// <summary>
/// Represents a connection entity between two tiles.
/// </summary>
public class ConnectionEntity : GameEntity
{
    public ConnectionEntity(EntityId id, string definitionId, string name, string category)
        : base(id, name, category, definitionId) 
    {
    }
}
