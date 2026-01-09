

// Namespace nou

using TurnForge.Engine.Entities.Traits.Interfaces;

namespace TurnForge.Engine.Entities.Traits.Standard;
public class VitalityTrait(int maxHp = 1, bool immortal = false) : ITrait
{
    public readonly int BaseMaxHp = maxHp;
    public bool IsImmortal { get; set; } = immortal;
}