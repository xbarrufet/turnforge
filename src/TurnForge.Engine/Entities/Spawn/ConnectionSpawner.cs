using TurnForge.Engine.Components;
using TurnForge.Engine.Entities; // For GameEntity
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Spawn;

/// <summary>
/// Spawns ConnectionEntities from ConnectionDescriptors defined in MissionData.
/// Each connection becomes a GameEntity with:
/// - Position: ConnectionPosition(From, To)
/// - TeamComponent if RestrictedToTeam is set
/// - Category stored in DefinitionId
/// </summary>
public sealed class ConnectionSpawner
{
    /// <summary>
    /// Create GameEntities from connection descriptors.
    /// </summary>
    public IEnumerable<GameEntity> CreateConnections(IEnumerable<ConnectionDescriptor> descriptors)
    {
        foreach (var descriptor in descriptors)
        {
            yield return CreateConnectionEntity(descriptor);
        }
    }
    
    /// <summary>
    /// Create GameEntities and record them directly to overlay.
    /// </summary>
    public void SpawnConnections(
        IEnumerable<ConnectionDescriptor> descriptors, 
        GameStateOverlay overlay)
    {
        foreach (var entity in CreateConnections(descriptors))
        {
            var operation = new SpawnEntityOperation(entity);
            overlay.Record(operation);
        }
    }
    
    private GameEntity CreateConnectionEntity(ConnectionDescriptor descriptor)
    {
        var entityId = EntityId.New();
        var position = ConnectionPosition.Between(descriptor.From, descriptor.To);
        
        // Build definition ID from category
        var definitionId = descriptor.DefinitionId 
            ?? $"connection_{descriptor.Category}_{descriptor.From}_{descriptor.To}";
        
        // Create concrete entity
        var entity = new ConnectionEntity(
            entityId,
            definitionId,
            name: definitionId,
            category: descriptor.Category
        );
        
        // Add Position Component
        var posComponent = new BasePositionComponent { CurrentPosition = position };
        entity.AddComponent(posComponent);
        
        // Add TeamComponent if restricted
        if (!string.IsNullOrEmpty(descriptor.RestrictedToTeam))
        {
            var trait = new TeamTrait(team: descriptor.RestrictedToTeam, controller: "System");
            var teamComponent = new TeamComponent(trait);
            entity.AddComponent(teamComponent);
        }
        
        return entity;
    }
}

