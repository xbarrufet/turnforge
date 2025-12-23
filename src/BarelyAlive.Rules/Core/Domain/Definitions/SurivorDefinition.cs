using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Components.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Definitions;

public class SurvivorDefinition : BaseGameEntityDefinition
{
    public SurvivorDefinition(string definitionId, string name, int maxHealth) : base(definitionId, name, "Survivor")
    {
        MaxHealth = maxHealth;
    }

    [MapToComponentAttribute(typeof(IHealthComponent), nameof(IHealthComponent.MaxHealth))]
    public int MaxHealth { get; set; }
}