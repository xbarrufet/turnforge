using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions; // For MissionDefinition
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay;

public interface IGameStateMutator
{
    // ─────────────────────────────
    // Entidades
    // ─────────────────────────────

    void AddEntity(GameEntity gameEntity);
    void RemoveEntity(EntityId entityId);

    // ─────────────────────────────
    // Componentes / datos
    // ─────────────────────────────

    void SetComponent<TComponent>(
        EntityId entityId,
        TComponent component)
        where TComponent : notnull;

    void RemoveComponent<TComponent>(EntityId entityId)
        where TComponent : notnull;

    // ─────────────────────────────
    // Posicionamiento / board
    // ─────────────────────────────

    void SetPosition(EntityId entityId, IBoardPosition position);
    void ClearPosition(EntityId entityId);

    // ─────────────────────────────
    // Mission / Board
    // ─────────────────────────────
    void SetMission(MissionDefinition mission);
    void SetBoard(IGameBoard board);

    // ─────────────────────────────
    // Estado de vida / flags comunes
    // (opcional pero práctico)
    // ─────────────────────────────

    void MarkDestroyed(EntityId entityId);
    
    // ─────────────────────────────
    // Turn Order
    // ─────────────────────────────
    void SetTurnOrder(TurnOrderState turnOrder);
}

