using BarelyAlive.Rules.Apis.Messaging;
using BarelyAlive.Rules.Core.Domain.Projectors.Interfaces;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Projectors.Handlers;

/// <summary>
/// Projects movement effects to domain events and state updates.
/// NOTE: Currently MoveCommand returns decisions (not effects) so this projector
/// may not fire until movement strategy generates specific effects.
/// This is a placeholder for future movement effect handling.
/// </summary>
public class AgentMovedProjector : IEffectProjector
{
    public bool CanHandle(IGameEffect effect)
    {
        // For now, we don't have a specific movement effect
        // This will be updated when movement generates proper effects
        return false;
    }

    public void Project(IGameEffect effect, 
        ICollection<EntityBuildUpdate> created,
        ICollection<EntityStateUpdate> updated, 
        ICollection<DomainEvent> events)
    {
        // Placeholder - will implement when we have movement effects
        // For now, movement state changes will be detected via GetGameState() queries
    }
}
