using System.Collections.Immutable;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Core.Orchestrator.Interfaces;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameState
{
    public ImmutableDictionary<EntityId, Agent> Agents { get; }
    public ImmutableDictionary<EntityId, Prop> Props { get; }
    public NodeId? CurrentStateId { get; }
    public GameBoard? Board { get; }
    public IScheduler Scheduler { get; }
    public ImmutableDictionary<string, object> Metadata { get; }

    private GameState(
        ImmutableDictionary<EntityId, Agent> agents,
        ImmutableDictionary<EntityId, Prop> props,
        NodeId? currentStateId,
        GameBoard? board,
        IScheduler scheduler,
        ImmutableDictionary<string, object> metadata)
    {
        Agents = agents;
        Props = props;
        CurrentStateId = currentStateId;
        Board = board;
        Scheduler = scheduler;
        Metadata = metadata;
    }

    public static GameState Empty()
        => new(
            ImmutableDictionary<EntityId, Agent>.Empty,
            ImmutableDictionary<EntityId, Prop>.Empty,
            null,
            null,
            TurnScheduler.Empty,
            ImmutableDictionary<string, object>.Empty
        );

    public GameState WithBoard(GameBoard board)
        => new(Agents, Props, CurrentStateId, board, Scheduler, Metadata);

    public GameState WithAgent(Agent agent)
        => new(
            Agents.Add(agent.Id, agent),
            Props,
            CurrentStateId,
            Board,
            Scheduler,
            Metadata
        );

    public GameState WithProp(Prop prop)
        => new(
            Agents,
            Props.Add(prop.Id, prop),
            CurrentStateId,
            Board,
            Scheduler,
            Metadata
        );

    public GameState WithAgents(IEnumerable<Agent> agents, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<EntityId, Agent>();
            foreach (var u in agents)
                builder[u.Id] = u;
            return new(builder.ToImmutable(), Props, CurrentStateId, Board, Scheduler, Metadata);
        }

        var updated = Agents;
        foreach (var u in agents)
            updated = updated.SetItem(u.Id, u);

        return new(updated, Props, CurrentStateId, Board, Scheduler, Metadata);
    }

    public GameState WithProps(IEnumerable<Prop> props, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<EntityId, Prop>();
            foreach (var u in props)
                builder[u.Id] = u;
            return new(Agents, builder.ToImmutable(), CurrentStateId, Board, Scheduler, Metadata);
        }

        var updated = Props;
        foreach (var u in props)
            updated = updated.SetItem(u.Id, u);

        return new(Agents, updated, CurrentStateId, Board, Scheduler, Metadata);
    }

    public GameState WithCurrentStateId(NodeId stateId)
        => new(Agents, Props, stateId, Board, Scheduler, Metadata);

    public GameState WithScheduler(IScheduler scheduler)
        => new(Agents, Props, CurrentStateId, Board, scheduler, Metadata);

    public GameState WithMetadata(string key, object value)
        => new(Agents, Props, CurrentStateId, Board, Scheduler, Metadata.SetItem(key, value));

    public IReadOnlyList<Agent> GetAgents() => Agents.Values.ToList();
    public IReadOnlyList<Prop> GetProps() => Props.Values.ToList();
}





