using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Definitions;
using TurnForge.Engine.Entities.Players;
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
    // Mission / Board
    // ─────────────────────────────
    // void SetMission(MissionDefinition mission);
    void SetBoard(IGameBoard board);

    // ─────────────────────────────
    // Turn Order
    // ─────────────────────────────
    void SetTurnOrder(TurnOrderState turnOrder);
    
    // ─────────────────────────────
    // Players
    // ─────────────────────────────
    void AddOrUpdatePlayer(Player player);
    
}

