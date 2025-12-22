using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Components.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Definitions;

public class SurvivorDefinition : GameEntityDefinition
{
    [MapToComponentAttribute(typeof(IHealthComponent), nameof(IHealthComponent.MaxHealth))]
    public int MaxHealth { get; set; }
}