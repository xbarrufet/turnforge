using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;
using TurnForge.Engine.Events;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

/// <summary>
/// Projects movement effects to domain events and state updates.
/// NOTE: Currently MoveCommand returns decisions (not effects) so this projector
/// may not fire until movement strategy generates specific effects.
/// This is a placeholder for future movement effect handling.
/// </summary>
public class AgentMovedProjector : IEventProjector
{
    public bool CanHandle(IGameEvent gameEvent)
    {
        // For now, we don't have a specific movement effect
        // This will be updated when movement generates proper events
        return false;
    }

    public void Project(IGameEvent gameEvent, 
        ICollection<EntityBuildUpdate> created,
        ICollection<EntityStateUpdate> updated, 
        ICollection<DomainEvent> events)
    {
        // Placeholder - will implement when we have movement effects
        // For now, movement state changes will be detected via GetGameState() queries
    }
}
