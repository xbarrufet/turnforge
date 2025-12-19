using System.Collections.Immutable;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameState
{
    public ImmutableDictionary<ActorId, Agent> Agents { get; }
    public ImmutableDictionary<ActorId, Prop> Props { get; }
    public NodeId? CurrentStateId { get; }

    private GameState(
        ImmutableDictionary<ActorId, Agent> agents,
        ImmutableDictionary<ActorId, Prop> props,
        NodeId? currentStateId)
    {
        Agents = agents;
        Props = props;
        CurrentStateId = currentStateId;
    }

    public static GameState Empty()
        => new(
            ImmutableDictionary<ActorId, Agent>.Empty,
            ImmutableDictionary<ActorId, Prop>.Empty,
            null
        );

    public GameState WithAgent(Agent agent)
        => new(
            Agents.Add(agent.Id, agent),
            Props,
            CurrentStateId
        );

    public GameState WithProp(Prop prop)
        => new(
            Agents,
            Props.Add(prop.Id, prop),
            CurrentStateId
        );

    public GameState WithAgents(IEnumerable<Agent> agents, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<ActorId, Agent>();
            foreach (var u in agents)
                builder[u.Id] = u;
            return new(builder.ToImmutable(), Props, CurrentStateId);
        }

        var updated = Agents;
        foreach (var u in agents)
            updated = updated.SetItem(u.Id, u);

        return new(updated, Props, CurrentStateId);
    }

    public GameState WithProps(IEnumerable<Prop> props, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<ActorId, Prop>();
            foreach (var u in props)
                builder[u.Id] = u;
            return new(Agents, builder.ToImmutable(), CurrentStateId);
        }

        var updated = Props;
        foreach (var u in props)
            updated = updated.SetItem(u.Id, u);

        return new(Agents, updated, CurrentStateId);
    }

    public GameState WithCurrentStateId(NodeId stateId)
        => new(Agents, Props, stateId);

    public IReadOnlyList<Agent> GetAgents() => Agents.Values.ToList();
    public IReadOnlyList<Prop> GetProps() => Props.Values.ToList();
}





