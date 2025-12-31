using TurnForge.Engine.Traits; // Namespace nou
namespace TurnForge.Engine.Traits.Standard;
public class VitalityTrait(int maxHP = 1, bool immortal = false) : BaseDataTrait
{
    public readonly int BaseMaxHP = maxHP;
    public bool IsImmortal { get; set; } = immortal;
}