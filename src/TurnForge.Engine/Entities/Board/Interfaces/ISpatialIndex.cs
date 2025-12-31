using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Domain.Board.Spatial.Interfaces;

public interface ISpatialIndex
{
    // ─────────────────────────────
    // Lifecycle / synchronization
    // ─────────────────────────────

    void Register(EntityId entityId, IBoardPosition position);
    void Update(EntityId entityId, IBoardPosition newPosition);
    void Unregister(EntityId entityId);
    IBoardPosition? GetEntityPosition(EntityId entityId);

    // ─────────────────────────────
    // Point / region queries
    // ─────────────────────────────

    IReadOnlyCollection<EntityId> QueryAt(IBoardPosition position);

    // ─────────────────────────────
    // Traversal queries (movement, LOS, charge, etc.)
    // ─────────────────────────────

    /*IReadOnlyCollection<EntityId> QueryTraversal(
        IBoardPosition from,
        IBoardPosition to,
        TraversalContext context);*/

    // ─────────────────────────────
    // Area queries (AOE, auras, zones)
    // ─────────────────────────────

    /*IReadOnlyCollection<EntityId> QueryArea(AreaQuery query);*/

    ISpatialIndex Clone();
}
