using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.TraitsComponents.Traits;

/// <summary>
/// Base class for Traits that are backed by a Runtime Component.
/// T is the Component type that handles this trait's logic.
/// </summary>
public abstract class BaseComponentTrait<T> : ITrait 
    where T : IGameEntityComponent
{
    
}