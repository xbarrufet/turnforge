using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
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
    
    public GameStateQueryService(GameState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
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
}
