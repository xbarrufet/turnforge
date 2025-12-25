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
    /// Get a prop by its EntityId string.
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
}
