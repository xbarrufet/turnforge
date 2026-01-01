using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Infrastructure.Factories;

public sealed class PlayerFactory : IPlayerFactory
{
    public Player BuildPlayer(PlayerDescriptor descriptor)
    {
        // Player construction logic
        // Since Player is no longer a GameEntity, we construct it directly.
        // Assuming Player has a constructor that takes PlayerId and Name/DefinitionId.
        
        var player = new Player(
             descriptor.PlayerId,
             descriptor.DefinitionId
        );

        // TODO: Handle Traits/Components if Player still supports them?
        // User said "remove player inheritance of GameEntity".
        // If Player needs logic (ActionPool), it might be a property now.
        // For now, simple construction. Needs verification of Player.cs changes.
        
        return player;
    }
}
