namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Marker trait indicating an entity can be the target of actions.
/// Used by Props (doors, levers, chests, explosive barrels).
/// The entity can receive actions on it (open, activate, destroy).
/// </summary>
public class ActionableTrait : BaseDataTrait
{
}
