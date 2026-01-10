using System.Net.NetworkInformation;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions.Actors;

public abstract class ActorDefinition : BaseGameEntityDefinition
{
    protected ActorDefinition(string definitionId, Category category) : base(definitionId, category)
    {
        // Add only Traits (configuration)
        // Components will be created automatically by ComponentInitializationService
        AddTrait(new VitalityTrait(), true);
        // MovableTrait removed - CurrentPosition is now a direct property on Actor
    }


}