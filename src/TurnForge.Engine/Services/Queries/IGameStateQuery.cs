using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Services.Queries;

/// <summary>
/// Query service for read-only access to game state.
/// Shared by strategies (backend) and UI projectors (Godot).
/// </summary>
/// <remarks>
/// Design Decision: Extract queries into a separate service to:
/// - Avoid duplication between strategy helpers and UI queries
/// - Enable testing with mocks
/// - Centralize query optimization (caching, indexing)
/// - Maintain single source of truth for state queries
/// </remarks>
public interface IGameStateQuery
{
    /// <summary>
    /// Get agent by ID string.
    /// </summary>
    /// <param name="agentId">String representation of EntityId</param>
    /// <returns>Agent if found, null otherwise</returns>
    Agent? GetAgent(string agentId);
    
    /// <summary>
    /// Get all agents in current state.
    /// </summary>
    IReadOnlyList<Agent> GetAllAgents();
    
    /// <summary>
    /// Get all agents at a specific position.
    /// Useful for occupancy checks, zombie counting, etc.
    /// </summary>
    IReadOnlyList<Agent> GetAgentsAt(Position position);
    
    /// <summary>
    /// Get all agents with a specific category (e.g., "Survivor", "Zombie").
    /// </summary>
    IReadOnlyList<Agent> GetAgentsByCategory(string category);
    
    /// <summary>
    /// Get all agents belonging to a specific team/faction.
    /// </summary>
    /// <param name="team">Team name (e.g., "Survivors", "Orcs")</param>
    IReadOnlyList<Agent> GetAgentsByTeam(string team);
    
    /// <summary>
    /// Get all agents controlled by a specific player/AI.
    /// </summary>
    /// <param name="controllerId">Controller ID (e.g., "Player1", "AI")</param>
    IReadOnlyList<Agent> GetAgentsByController(string controllerId);
    /// </summary>
    Prop? GetProp(string propId);
    
    /// <summary>
    /// Get all props at a specific position.
    /// </summary>
    IReadOnlyList<Prop> GetPropsAt(Position position);
    
    /// <summary>
    /// Check if any agent occupies the position.
    /// </summary>
    bool IsPositionOccupied(Position position);
    
    /// <summary>
    /// Count agents of specific category at position.
    /// Common use: count zombies for movement cost calculation.
    /// </summary>
    int CountAgentsAt(Position position, string? category = null);
    
    // ────────────────────────────────────────────────────────────
    // Action Queries (UI Preview)
    // ────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Get all valid move destinations for an agent.
    /// Used by UI to preview available moves before execution.
    /// </summary>
    /// <param name="agentId">Agent ID to query</param>
    /// <returns>List of valid positions the agent can move to. Empty if agent not found or no valid moves.</returns>
    /// <remarks>
    /// This performs simplified validation (board bounds, AP check) without replicating
    /// full strategy logic. Good for 80% of UI preview use cases.
    /// Does NOT account for game-specific rules like zombie tax.
    /// </remarks>
    IReadOnlyList<Position> GetValidMoveDestinations(string agentId);
    
    /// <summary>
    /// Get all valid combat targets for an agent.
    /// Used by UI to preview available attack targets before execution.
    /// </summary>
    /// <param name="agentId">Agent ID to query</param>
    /// <returns>List of agents that can be attacked. Empty if no valid targets.</returns>
    /// <remarks>
    /// Current implementation: Returns agents in adjacent tiles with a different category.
    /// Does NOT check weapon range or line of sight (future enhancement).
    /// </remarks>
    IReadOnlyList<Agent> GetValidCombatTargets(string agentId);
    
    /// <summary>
    /// Checks if all agents in a specific category have consumed all their action points.
    /// </summary>
    /// <param name="category">Category to check (e.g., "Survivor", "Zombie")</param>
    /// <returns>True if all agents in category have 0 AP remaining, or if no agents exist in category</returns>
    /// <remarks>
    /// Used by FSM nodes to determine if a faction's turn is complete.
    /// Returns true if no agents exist in the category (vacuous truth).
    /// </remarks>
    bool IsAllAgentsActionPointsConsumed(string category);
    
    // ────────────────────────────────────────────────────────────
    // Item Queries
    // ────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Get an item by its EntityId string.
    /// </summary>
    /// <param name="itemId">String representation of EntityId</param>
    /// <returns>Item if found, null otherwise</returns>
    TurnForge.Engine.Entities.Items.Item? GetItem(string itemId);
    
    /// <summary>
    /// Get all items owned by an entity (Agent or Container).
    /// </summary>
    /// <param name="ownerId">Owner's EntityId string</param>
    /// <returns>List of items belonging to the owner</returns>
    IReadOnlyList<TurnForge.Engine.Entities.Items.Item> GetItemsByOwner(string ownerId);
}
