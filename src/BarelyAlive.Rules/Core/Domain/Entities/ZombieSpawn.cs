using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Core.Domain.Descriptors;

namespace BarelyAlive.Rules.Core.Domain.Entities;

/// <summary>
/// ZombieSpawn prop - spawn point for zombie waves.
/// </summary>
/// <remarks>
/// This entity has a custom component (ZombieSpawnComponent) with an Order property
/// that controls spawn wave priority. The descriptor allows type-safe spawn processing.
/// </remarks>
[DefinitionType(typeof(ZombieSpawnDefinition))]
[DescriptorType(typeof(ZombieSpawnDescriptor))]
public class ZombieSpawn : Prop
{
    public ZombieSpawn(EntityId id, string definitionId, string name, string category) 
        : base(id, definitionId, name, category)
    {
        AddComponent(new ZombieSpawnComponent());
    }
}

/// <summary>
/// Component for ZombieSpawn entities that stores spawn wave order.
/// </summary>
public class ZombieSpawnComponent : IGameEntityComponent
{
    /// <summary>
    /// Spawn order for zombie waves (lower values spawn first).
    /// </summary>
    public int Order { get; set; } = 1;

    public ZombieSpawnComponent() { }

    public ZombieSpawnComponent(BarelyAlive.Rules.Core.Domain.Traits.SpawnOrderTrait trait)
    {
        Order = trait.Order;
    }
}

/// <summary>
/// Definition for ZombieSpawn props.
/// </summary>
public class ZombieSpawnDefinition : PropDefinition
{
    public int Order { get; set; } = 1;
    
    public ZombieSpawnDefinition(string definitionId) 
        : base(definitionId)
    {
    }
}