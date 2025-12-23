using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Tests.Helpers;

/// <summary>
/// Test definition for Prop entities - used in tests only
/// </summary>
public class TestPropDefinition : BaseGameEntityDefinition
{
    [MapToComponent(typeof(IPositionComponent), nameof(IPositionComponent.CurrentPosition))]
    public IPositionComponent? PositionComponent { get; set; }

    [MapToComponent(typeof(IHealthComponent), nameof(IHealthComponent.MaxHealth))]
    public int MaxHealth { get; set; } = 1;

    public IReadOnlyList<IActorBehaviour> Behaviours { get; set; } = new List<IActorBehaviour>();
}
