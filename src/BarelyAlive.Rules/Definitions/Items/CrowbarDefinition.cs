using BarelyAlive.Rules.Definitions.Traits;
using TurnForge.Engine.Definitions.Items;
using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits.Standard.Checkers;
using TurnForge.Engine.Definitions;

namespace BarelyAlive.Rules.Definitions.Items;

public class CrowbarDefinition : ItemDefinition
{
    public CrowbarDefinition() : base("crowbar", "Cuerpo a cuerpo")
    {
        // Stats
        AddTrait(new WeaponRangeTrait.Builder().Range(0).Build());
        
        // Combat Traits
        AddTrait(new ToHitTrait.Builder()
            .Dice("2d6")
            .On(4)
            .Scope(new AnyOfThem())
            .Build());
        
        // Damage: 1 (Physical)
        AddTrait(new DamageTrait.Builder()
            .WithProfile("Standard", 1, "Physical")
            .Build());
            
        // Special Rules
        AddTrait(new InitialEquipmentTrait());
        AddTrait(new CanOpenDoorsTrait(requiresRoll: false));
    }
}
