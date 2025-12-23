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
    public SurvivorDefinition(string definitionId, string name) : base(definitionId, name, "Survivor")
    {
    }
}

[EntityType(typeof(Survivor))]
public class SurvivorDescription : AgentDescription {

}