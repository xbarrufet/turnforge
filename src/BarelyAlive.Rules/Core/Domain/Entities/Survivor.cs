using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Core.Domain.Entities;

public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name) : base(id, definitionId, name, "Survivor")
    {
    }

}

[EntityType(typeof(Survivor))]
public class SurvivorDefinition : BaseGameEntityDefinition
{
    [MapToComponent(typeof(IHealthComponent), TurnForgeComponents.Prop_HealthComponent_MaxHealth)]
    public int MaxHealth { get; set; }
    public SurvivorDefinition(string definitionId, string name) : base(definitionId, name, "Survivor")
    {
        
    }
}

[EntityType(typeof(Survivor))]
public class SurvivorDescription : AgentDescription {

}