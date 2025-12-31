using TurnForge.Engine.Traits.Standard;

namespace TurnForge.Engine.Entities.Definitions.Actors;

/// <summary>
/// Base definition for Prop entities.
/// Props are located actors that can be the target of actions (open, activate, destroy).
/// </summary>
public abstract class PropDefinition : ActorDefinition
{
    protected PropDefinition(string definitionId, string category) : base(definitionId, category)
    {
        AddTrait(new ActionableTrait());
    }
}