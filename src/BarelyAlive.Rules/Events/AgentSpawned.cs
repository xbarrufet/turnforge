
using BarelyAlive.Rules.Events.Interfaces;

namespace BarelyAlive.Rules.Events;

public sealed record AgentSpawned(string AgentId, string AgentType, string AreaId) : IBarelyAliveEffect;
