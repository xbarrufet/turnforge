using TurnForge.Engine.Traits; // Namespace nou
namespace TurnForge.Engine.Traits.Standard;
public class VitalityTrait : BaseTrait
{
    public int BaseMaxHP { get; set; }
    public bool IsImmortal { get; set; }
    // constructor per defecte (necessari per serialització)
    public VitalityTrait() { }
    // constructor "Fluent" per comoditat del dev
    public VitalityTrait(int maxHP, bool immortal = false)
    {
        BaseMaxHP = maxHP;
        IsImmortal = immortal;
    }
}