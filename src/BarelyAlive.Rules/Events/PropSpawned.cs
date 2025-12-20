using BarelyAlive.Rules.Events.Interfaces;

namespace BarelyAlive.Rules.Events;

public sealed record PropSpawned(string PropId, string PropType, string AreaId) : IBarelyAliveEffect
{
}