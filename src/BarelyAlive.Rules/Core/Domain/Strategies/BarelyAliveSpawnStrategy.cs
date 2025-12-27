using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Core.Domain.Descriptors;
using BarelyAlive.Rules.Core.Domain.Entities;

namespace BarelyAlive.Rules.Core.Domain.Strategies;

/// <summary>
/// BarelyAlive-specific spawn strategy using pattern matching for type dispatch.
/// </summary>
/// <remarks>
/// Uses C# pattern matching (switch expressions) to handle:
/// - SurvivorDescriptor: Assign to player spawn point
/// - Generic fallback: Accept as-is
/// 
/// NOTE: ZombieSpawnDescriptor batch processing removed as it was for Props,
/// not Agents. Zombie spawns are Props and should use PropSpawnStrategy.
/// </remarks>
public class BarelyAliveSpawnStrategy : BaseSpawnStrategy
{
    /// <summary>
    /// Override ProcessDescriptor to use pattern matching for type dispatch.
    /// </summary>
    /// <summary>
    /// Override ProcessDescriptor to use pattern matching for type dispatch.
    /// </summary>
    protected override AgentDescriptor ProcessDescriptor(
        AgentDescriptor descriptor,
        GameState state)
    {
        // Pattern matching: compile-time type dispatch (zero reflection)
        return descriptor switch
        {
            SurvivorDescriptor survivor => ProcessSurvivor(survivor, state),
            _ => ProcessGeneric(descriptor, state) // Fallback
        };
    }

    // ─────────────────────────────────────────────────────────────
    // Type-specific handlers
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Process Survivor: assign to player spawn point and set faction.
    /// </summary>
    private SurvivorDescriptor ProcessSurvivor(
        SurvivorDescriptor descriptor,
        GameState state)
    {
        // Find player spawn point
        var playerSpawn = state.GetProps()
            .FirstOrDefault(p => p.Category == "Spawn.Player");

        // Handle Position
        var positionTrait = descriptor.RequestedTraits
            .OfType<TurnForge.Engine.Traits.Standard.PositionTrait>()
            .FirstOrDefault();

        // If no explicit position requested, try to use spawn point or fallback
        if (positionTrait == null || positionTrait.InitialPosition == Position.Empty)
        {
            if (playerSpawn?.PositionComponent != null)
            {
                // Add/Update PositionTrait with spawn point
                AddOrUpdateTrait(descriptor, new TurnForge.Engine.Traits.Standard.PositionTrait(playerSpawn.PositionComponent.CurrentPosition));
            }
        }

        // Handle Faction (Team)
        var teamTrait = descriptor.RequestedTraits
            .OfType<TurnForge.Engine.Traits.Standard.TeamTrait>()
            .FirstOrDefault();
            
        if (teamTrait == null)
        {
             // Default to "Player" team
             AddOrUpdateTrait(descriptor, new TurnForge.Engine.Traits.Standard.TeamTrait("Player", "Player"));
        }
        
        // Handle Action Points
        var apTrait = descriptor.RequestedTraits
            .OfType<TurnForge.Engine.Traits.Standard.ActionPointsTrait>()
            .FirstOrDefault();

        if (apTrait == null)
        {
            // Default 3 AP for Survivors
            AddOrUpdateTrait(descriptor, new TurnForge.Engine.Traits.Standard.ActionPointsTrait(3));
        }

        return descriptor;
    }

    /// <summary>
    /// Generic fallback: accept descriptor as-is.
    /// </summary>
    private AgentDescriptor ProcessGeneric(
        AgentDescriptor descriptor,
        GameState state)
    {
        // Heuristic: If definition starts with "Zombie.", give 1 AP
        if (descriptor.DefinitionId.StartsWith("Zombie."))
        {
             AddOrUpdateTrait(descriptor, new TurnForge.Engine.Traits.Standard.ActionPointsTrait(1));
        }
        else if (descriptor.DefinitionId.StartsWith("Survivor."))
        {
            // Fallback for generic AgentDescriptor for Survivor
             AddOrUpdateTrait(descriptor, new TurnForge.Engine.Traits.Standard.ActionPointsTrait(3));
        }
        
        return descriptor;
    }

    private void AddOrUpdateTrait<T>(AgentDescriptor descriptor, T trait) where T : TurnForge.Engine.Traits.Interfaces.IBaseTrait
    {
        // Remove existing of same type
        var existing = descriptor.RequestedTraits
            .FirstOrDefault(t => t.GetType() == trait.GetType());
            
        if (existing != null)
        {
            descriptor.RequestedTraits.Remove(existing);
        }
        
        descriptor.RequestedTraits.Add(trait);
    }
}
