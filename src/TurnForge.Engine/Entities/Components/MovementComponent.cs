using System.ComponentModel;
using TurnForge.Engine.Entities.Components.Definitions;
using TurnForge.Engine.Entities.Components.Interfaces;

namespace TurnForge.Engine.Entities.Components;

public sealed class MovementComponent(MovementComponentDefinition definition):IGameEntityComponent
{
    public MovementComponentDefinition Definition { get; } = definition;
    public int MaxUnitsToMove => Definition.MaxUnitsToMove;
    
}