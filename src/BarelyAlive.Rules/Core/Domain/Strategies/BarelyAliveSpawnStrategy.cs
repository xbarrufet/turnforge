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

        // Assign position if found
        if (playerSpawn?.PositionComponent != null)
        {
            descriptor.Position = playerSpawn.PositionComponent.CurrentPosition;
        }
        else
        {
            // Fallback: keep position from request or set to empty
            if (descriptor.Position == Position.Empty)
            {
                descriptor.Position = Position.Empty;
            }
        }

        // Ensure faction is set to Player (can be overridden from SpawnRequest)
        if (string.IsNullOrEmpty(descriptor.Faction))
        {
            descriptor.Faction = "Player";
        }
        
        // Add Action Points component
        descriptor.ExtraComponents.Add(new TurnForge.Engine.Components.BaseActionPointsComponent(descriptor.ActionPoints)
        {
             CurrentActionPoints = descriptor.ActionPoints
        });

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
        if (descriptor.DefinitionID.StartsWith("Zombie."))
        {
            descriptor.ExtraComponents.Add(new TurnForge.Engine.Components.BaseActionPointsComponent(1)
            {
                 CurrentActionPoints = 1
            });
        }
        else if (descriptor.DefinitionID.StartsWith("Survivor."))
        {
            // Fallback for generic AgentDescriptor for Survivor
            descriptor.ExtraComponents.Add(new TurnForge.Engine.Components.BaseActionPointsComponent(3)
            {
                 CurrentActionPoints = 3
            });
        }
        
        // Accept as-is - no modifications needed
        return descriptor;
    }
}
