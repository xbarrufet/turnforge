using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Traits;

/// <summary>
/// Base class for Traits that are backed by a Runtime Component.
/// T is the Component type that handles this trait's logic.
/// </summary>
public abstract class BaseComponentTrait<T> : BaseTrait 
    where T : IGameEntityComponent
{
    // Aquesta classe actua com a "Pont" entre Dades (Trait) i Lògica (Component).
    // De moment no necessita res més, el tipus T ja fa la feina dura.
}