
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Actors;

/// <summary>
/// Base definition for Prop entities.
/// Props are located actors that can be the target of actions (open, activate, destroy).
/// </summary>
public abstract class PropDefinition : ActorDefinition
{
    protected PropDefinition(string definitionId, Category category) : base(definitionId, category)
    {
       
    }
    
    protected PropDefinition(string definitionId) : base(definitionId, Prop.PropDefaultCategory)
    {
       
    }
}