namespace TurnForge.Engine.Traits.Interfaces;

/// <summary>
/// Marker interface for traits that contain only data.
/// These traits do not react to events.
/// Examples: HealthTrait, MovementTrait, IdentityTrait
/// </summary>
public interface IDataTrait : IBaseTrait
{
}
