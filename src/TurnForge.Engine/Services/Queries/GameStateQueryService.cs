using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Services.Queries;

/// <summary>
/// Default implementation of IGameStateQuery.
/// </summary>
/// <remarks>
/// Wraps GameState and provides optimized query methods.
/// Future: Can add caching, indexing, or other optimizations here.
/// </remarks>
public sealed class GameStateQueryService : IGameStateQuery
{
    private readonly GameState _state;
    private readonly GameBoard _board;
    
    public GameStateQueryService(GameState state, GameBoard board)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _board = board ?? throw new ArgumentNullException(nameof(board));
    }
    
    public Agent? GetAgent(string agentId)
    {
        if (string.IsNullOrWhiteSpace(agentId))
            return null;
        
        return _state.GetAgents()
            .FirstOrDefault(a => a.Id.ToString() == agentId);
    }
    
    public IReadOnlyList<Agent> GetAllAgents()
    {
        return _state.GetAgents().ToList();
    }
    
    public IReadOnlyList<Agent> GetAgentsAt(Position position)
    {
        if (position == Position.Empty)
            return Array.Empty<Agent>();
        
        return _state.GetAgents()
            .Where(a => a.PositionComponent.CurrentPosition == position)
            .ToList();
    }
    
    public IReadOnlyList<Agent> GetAgentsByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return Array.Empty<Agent>();
        
        return _state.GetAgents()
            .Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    public IReadOnlyList<Agent> GetAgentsByTeam(string team)
    {
        if (string.IsNullOrWhiteSpace(team))
            return Array.Empty<Agent>();
        
        return _state.GetAgents()
            .Where(a => a.Team.Equals(team, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    public IReadOnlyList<Agent> GetAgentsByController(string controllerId)
    {
        if (string.IsNullOrWhiteSpace(controllerId))
            return Array.Empty<Agent>();
        
        return _state.GetAgents()
            .Where(a => controllerId.Equals(a.ControllerId, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    public bool IsPositionOccupied(Position position)
    {
        if (position == Position.Empty)
            return false;
        
        return _state.GetAgents()
            .Any(a => a.PositionComponent.CurrentPosition == position);
    }
    
    public int CountAgentsAt(Position position, string? category = null)
    {
        if (position == Position.Empty)
            return 0;
        
        var agents = _state.GetAgents()
            .Where(a => a.PositionComponent.CurrentPosition == position);
        
        if (!string.IsNullOrWhiteSpace(category))
            agents = agents.Where(a => a.Category == category);
        
        return agents.Count();
    }
    
    public Prop? GetProp(string propId)
    {
        if (string.IsNullOrWhiteSpace(propId))
            return null;
        
        return _state.GetProps()
            .FirstOrDefault(p => p.Id.ToString() == propId);
    }
    
    public IReadOnlyList<Prop> GetPropsAt(Position position)
    {
        if (position == Position.Empty)
            return Array.Empty<Prop>();
        
        return _state.GetProps()
            .Where(p => p.PositionComponent.CurrentPosition == position)
            .ToList();
    }
    
    // ────────────────────────────────────────────────────────────
    // Action Queries (UI Preview)
    // ────────────────────────────────────────────────────────────
    
    public IReadOnlyList<Position> GetValidMoveDestinations(string agentId)
    {
        // 1. Get agent
        var agent = GetAgent(agentId);
        if (agent == null) 
            return Array.Empty<Position>();
        
        // 2. Check AP available
        var apComponent = agent.GetComponent<TurnForge.Engine.Components.Interfaces.IActionPointsComponent>();
        var availableAP = apComponent?.CurrentActionPoints ?? 0;
        if (availableAP <= 0) 
            return Array.Empty<Position>();
        
        // 3. Get current position
        var currentPos = agent.PositionComponent.CurrentPosition;
        
        // 4. Get neighbors from board
        var neighbors = _board.GetNeighbors(currentPos);
        
        // 5. Filter valid moves
        return neighbors
            .Where(pos => IsValidMoveDestination(agent, currentPos, pos))
            .ToList();
    }
    
    private bool IsValidMoveDestination(Agent agent, Position from, Position to)
    {
        // Basic validation:
        // - Position is valid on board
        // - Position is different from current
        // - Can afford basic move cost (1 AP)
        
        if (!_board.IsValid(to)) 
            return false;
        
        if (from == to) 
            return false;
        
        // For now, assume 1 AP per move (simplified)
        // Future: could check for zombie tax, but that's BarelyAlive-specific
        var apComponent = agent.GetComponent<TurnForge.Engine.Components.Interfaces.IActionPointsComponent>();
        if (apComponent == null) 
            return true; // No AP system = always valid
        
        return apComponent.CurrentActionPoints >= 1;
    }
    
    // ────────────────────────────────────────────────────────────
    // Turn/Faction Queries
    // ────────────────────────────────────────────────────────────
    
    public bool IsAllAgentsActionPointsConsumed(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return true; // No category = considered done
        
        var agents = _state.GetAgents()
            .Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        if (agents.Count == 0)
            return true; // No agents in category = considered done (vacuous truth)
        
        return agents.All(agent =>
        {
            var apComponent = agent.GetComponent<TurnForge.Engine.Components.Interfaces.IActionPointsComponent>();
            if (apComponent == null)
                return true; // No AP component = considered done
            
            return apComponent.CurrentActionPoints <= 0;
        });
    }
    
    // ────────────────────────────────────────────────────────────
    // Item Queries
    // ────────────────────────────────────────────────────────────
    
    public Entities.Items.Item? GetItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return null;
        
        return _state.GetItems()
            .FirstOrDefault(i => i.Id.ToString() == itemId);
    }
    
    public IReadOnlyList<Entities.Items.Item> GetItemsByOwner(string ownerId)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            return Array.Empty<Entities.Items.Item>();
        
        return _state.GetItems()
            .Where(i => i.ItemComponent.OwnerId?.ToString() == ownerId)
            .ToList();
    }
    
    // ────────────────────────────────────────────────────────────
    // Combat Queries
    // ────────────────────────────────────────────────────────────
    
    public IReadOnlyList<Agent> GetValidCombatTargets(string agentId)
    {
        // 1. Get agent
        var agent = GetAgent(agentId);
        if (agent == null) 
            return Array.Empty<Agent>();
        
        // 2. Get agent position and team
        var currentPos = agent.PositionComponent.CurrentPosition;
        var myTeam = agent.Team;
        
        // 3. Get neighbors from board
        var neighbors = _board.GetNeighbors(currentPos);
        
        // 4. Find agents in adjacent positions with different team
        var targets = new List<Agent>();
        foreach (var neighborPos in neighbors)
        {
            var agentsAtPos = GetAgentsAt(neighborPos);
            foreach (var potentialTarget in agentsAtPos)
            {
                // Different team = valid target (empty team = neutral, attackable)
                if (string.IsNullOrEmpty(myTeam) || 
                    string.IsNullOrEmpty(potentialTarget.Team) ||
                    !potentialTarget.Team.Equals(myTeam, StringComparison.OrdinalIgnoreCase))
                {
                    targets.Add(potentialTarget);
                }
            }
        }
        
        return targets;
    }
}
