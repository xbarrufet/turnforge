using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Entities.Traits.Interfaces;

public interface IComponentTrait<out TGameEntityComponent> : ITrait 
    where TGameEntityComponent : IGameEntityComponent
{
    Type SupportedComponentType { get; }
}
