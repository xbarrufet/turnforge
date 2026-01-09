using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.TraitsComponents.Traits;

public class ZoneBoundTrait:BaseTrait
{
    public IZoneTopology ZoneTopology { get; }
    public ZoneId ZoneId { get; }

    public ZoneBoundTrait()
    {
        IsInitialized = false;
        ZoneId = ZoneId.Empty;
        ZoneTopology = IZoneTopology.Empty;
    }
    public ZoneBoundTrait(ZoneId zoneId,IZoneTopology zoneTopology)
    {
        ZoneTopology = zoneTopology;
        ZoneId = zoneId;
        IsInitialized = true;
    }
    
}