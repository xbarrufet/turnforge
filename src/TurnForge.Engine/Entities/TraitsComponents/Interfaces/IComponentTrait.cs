namespace TurnForge.Engine.Entities.TraitsComponents.Interfaces;

public interface IComponentTrait<out TGameEntityComponent> : ITrait 
    where TGameEntityComponent : IGameEntityComponent
{
    Type SupportedComponentType { get; }
}
