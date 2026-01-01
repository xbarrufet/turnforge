using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Entities.Board;

public sealed class Zone : GameEntity
{
    public Zone(EntityId id, string definitionId, string name, string category) 
        : base(id, definitionId, name, category)
    {
    }

    // Logic relying on definition/bound refactored to use Component.
    public bool Contains(IBoardPosition position)
    {
        var boundComponent = GetComponent<TurnForge.Engine.Components.Board.IZoneBoundComponent>();
        if (boundComponent == null)
        {
            // Fallback or throw. A Zone MUST have a bound.
            throw new InvalidOperationException($"Zone {Id} has no Bound component.");
        }
        return boundComponent.Bound.Contains(position);
    }
}
