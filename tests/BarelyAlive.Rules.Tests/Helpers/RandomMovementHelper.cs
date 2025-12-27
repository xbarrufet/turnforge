using BarelyAlive.Rules.Game;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Tests.Helpers;

public static class RandomMovementHelper
{
    private static readonly Random _random = new Random(12345); // Seeded for deterministic tests
    
    /// <summary>
    /// Selects a random valid move destination for an agent.
    /// Returns null if no valid destinations exist.
    /// </summary>
    public static Position? GetRandomMoveDestination(
        TurnForge.Engine.Definitions.GameState state, 
        string agentId)
    {
        if (state.Board == null) return null;
        
        var query = new TurnForge.Engine.Services.Queries.GameStateQueryService(state, state.Board);
        var validDestinations = query.GetValidMoveDestinations(agentId);
        
        if (validDestinations.Count == 0)
            return null;
        
        var randomIndex = _random.Next(validDestinations.Count);
        return validDestinations[randomIndex];
    }
}
