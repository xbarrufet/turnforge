using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Entities;

public class ZombieSpawn : Prop
{
    public ZombieSpawn(EntityId id, string definitionId, string name, string category) : base(id, definitionId, name, category)
    {
        AddComponent(new ZombieSpawnComponent());
    }
}

public class ZombieSpawnComponent : IGameEntityComponent
{
    public int Order { get; set; }
}
[EntityType(typeof(ZombieSpawn))]
public class ZombieSpawnDescriptor(string definitionId) : PropDescriptor(definitionId)
{
    [MapToComponent(typeof(ZombieSpawnComponent), nameof(Order))]
    public int Order { get; set; }
}

[EntityType(typeof(ZombieSpawn))]
public class ZombieSpawnDefinition(string definitionId, string name, string category, int order) : PropDefinition(definitionId, name, category)
{
    [MapToComponent(typeof(ZombieSpawnComponent), nameof(Order))]
    public int Order { get; set; } = order;
}