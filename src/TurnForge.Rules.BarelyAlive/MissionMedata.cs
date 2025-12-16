
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Rules.BarelyAlive;

public sealed class MissionMetadata
{
    public string MissionName { get; }
    public Size MapSize { get; }

    public MissionMetadata(string missionName, Size mapSize)
    {
        MissionName = missionName;
        MapSize = mapSize;
    }
}