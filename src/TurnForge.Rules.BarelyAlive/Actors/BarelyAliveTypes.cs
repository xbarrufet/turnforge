using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Rules.BarelyAlive.Actors;

public static class BarelyAliveTypes
{
    public static readonly UnitTypeId Survivor =
        new("BarelyAlive.unit.survivor");

    public static readonly NpcTypeId Zombie =
        new("BarelyAlive.hostile.zombie");

    public static readonly PropTypeId ZombieSpawn =
        new("BarelyAlive.prop.zombie_spawn");
    
    public static readonly PropTypeId PartySpawn =
        new("BarelyAlive.prop.party_spawn");

    public static readonly PropTypeId Door =
        new("BarelyAlive.prop.door");
}
