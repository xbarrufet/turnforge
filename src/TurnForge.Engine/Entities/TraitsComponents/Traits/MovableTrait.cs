using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Entities.Traits.Interfaces;

namespace TurnForge.Engine.Entities.Traits;

public class MovableTrait : IComponentTrait<IMovementComponent>
{
    int MaxUnitsToMove { get; init; } = 1;
    public Type SupportedComponentType => typeof(IMovementComponent);
}