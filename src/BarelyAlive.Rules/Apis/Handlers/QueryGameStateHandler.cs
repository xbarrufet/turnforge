using BarelyAlive.Rules.Apis.Messaging;
using TurnForge.Engine.Repositories.Interfaces;

namespace BarelyAlive.Rules.Apis.Handlers;

/// <summary>
/// Handles queries for current game state.
/// Projects engine state to domain-specific DTOs with Survivors/Zombies separated.
/// </summary>
public class QueryGameStateHandler
{
    private readonly IGameRepository _gameRepository;

    public QueryGameStateHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public GameStateSnapshot Handle()
    {
        var state = _gameRepository.LoadGameState();
        
        var allAgents = state.GetAgents();
        
        // Separate agents by category (Survivor vs Zombie)
        var survivors = allAgents
            .Where(a => a.Category.Equals("Survivor", StringComparison.OrdinalIgnoreCase))
            .Select(a => new SurvivorDto
            {
                Id = a.Id.Value.ToString(),
                TypeId = a.DefinitionId,
                Position = new TileReference(a.PositionComponent.CurrentPosition.TileId.Value.ToString())
            })
            .ToList();

        var zombies = allAgents
            .Where(a => a.Category.Equals("Zombie", StringComparison.OrdinalIgnoreCase))
            .Select(a => new ZombieDto
            {
                Id = a.Id.Value.ToString(),
                TypeId = a.DefinitionId,
                Position = new TileReference(a.PositionComponent.CurrentPosition.TileId.Value.ToString())
            })
            .ToList();

        var props = state.GetProps().Select(p => new PropDto
        {
            Id = p.Id.Value.ToString(),
            TypeId = p.DefinitionId,
            Position = new TileReference(p.PositionComponent.CurrentPosition.TileId.Value.ToString())
        }).ToList();

        var board = state.Board != null 
            ? new BoardDto 
            {
                TileCount = state.GetProps().Count + allAgents.Count, // Rough estimate 
                ZoneCount = state.Board.Zones.Count
            }
            : null;

        return new GameStateSnapshot
        {
            Survivors = survivors,
            Zombies = zombies,
            Props = props,
            Board = board
        };
    }
}
