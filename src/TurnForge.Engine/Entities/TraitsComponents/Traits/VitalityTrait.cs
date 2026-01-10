

// Namespace nou

using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.TraitsComponents.Traits;
public class VitalityTrait : BaseTrait
{
    public int BaseMaxHp { get; init; }
    public bool IsImmortal { get; init; }

    public VitalityTrait()
    {
        BaseMaxHp = 1;
        IsImmortal = false;
    }
    
    public VitalityTrait(int baseMaxHp, bool isImmortal = false)
    {
        BaseMaxHp = baseMaxHp;
        IsImmortal = isImmortal;
        IsInitialized = true;
    }
}