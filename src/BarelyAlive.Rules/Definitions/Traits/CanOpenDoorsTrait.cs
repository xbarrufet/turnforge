using TurnForge.Engine.Traits;

namespace BarelyAlive.Rules.Definitions.Traits;

/// <summary>
/// Indicates the ability to open doors.
/// </summary>
public class CanOpenDoorsTrait : BaseTrait
{
    public bool RequiresRoll { get; }
    public bool Silent { get; } // Crowbar might be noisy? "sin tirada" often implies noise management in Zombicide.
    // User request: "Abre puertas (sin tirada)". Just focusing on roll for now.

    public CanOpenDoorsTrait(bool requiresRoll = true, bool silent = false)
    {
        RequiresRoll = requiresRoll;
        Silent = silent;
    }
}
