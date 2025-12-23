using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Tests.Helpers;

/// <summary>
/// Test definition for Agent entities - used in tests only
/// </summary>
public class TestAgentDefinition : BaseGameEntityDefinition
{
    [MapToComponent(typeof(IPositionComponent), nameof(IPositionComponent.CurrentPosition))]
    public IPositionComponent? PositionComponent { get; set; }

    [MapToComponent(typeof(IHealthComponent), nameof(IHealthComponent.MaxHealth))]
    public int MaxHealth { get; set; } = 1;

    public int MaxMovement { get; set; } = 1;

    public IReadOnlyList<IActorBehaviour> Behaviours { get; set; } = new List<IActorBehaviour>();
    
    public string AgentName { get; set; } = string.Empty;
}
