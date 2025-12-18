using System.Collections.Immutable;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameState
{
    public ImmutableDictionary<ActorId, Unit> Units { get; }
    public ImmutableDictionary<ActorId, Npc> Npcs { get; }
    public ImmutableDictionary<ActorId, Prop> Props { get; }

    private GameState(
        ImmutableDictionary<ActorId, Unit> units,
        ImmutableDictionary<ActorId, Prop> props,
        ImmutableDictionary<ActorId, Npc> npcs)
    {
        Units = units;
        Props = props;
        Npcs = npcs;
    }

    public static GameState Empty()
        => new(
            ImmutableDictionary<ActorId, Unit>.Empty,
            ImmutableDictionary<ActorId, Prop>.Empty,
            ImmutableDictionary<ActorId, Npc>.Empty
        );

    public GameState WithUnit(Unit unit)
        => new(
            Units.Add(unit.Id, unit),
            Props,
            Npcs
        );

    public GameState WithProp(Prop prop)
        => new(
            Units,
            Props.Add(prop.Id, prop),
            Npcs
        );
    public GameState WithNpc(Npc npc)
        => new(
            Units,
            Props,
            Npcs.Add(npc.Id, npc)
        );
    
    public GameState WithUnits(IEnumerable<Unit> units, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<ActorId, Unit>();
            foreach (var u in units)
                builder[u.Id] = u;
            return new(builder.ToImmutable(), Props, Npcs);
        }
    
        var updated = Units;
        foreach (var u in units)
            updated = updated.SetItem(u.Id, u);
    
        return new(updated, Props, Npcs);
    }
    
    public GameState WithProps(IEnumerable<Prop> props, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<ActorId, Prop>();
            foreach (var u in props)
                builder[u.Id] = u;
            return new(Units, builder.ToImmutable(), Npcs);
        }
    
        var updated = Props;
        foreach (var u in props)
            updated = updated.SetItem(u.Id, u);
    
        return new(Units, updated, Npcs);
    }
    
    public GameState WithNpcs(IEnumerable<Npc> npcs, bool replaceAll = false)
    {
        if (replaceAll)
        {
            var builder = ImmutableDictionary.CreateBuilder<ActorId, Npc>();
            foreach (var u in npcs)
                builder[u.Id] = u;
            return new(Units,Props, builder.ToImmutable());
        }
    
        var updated = Npcs;
        foreach (var u in npcs)
            updated = updated.SetItem(u.Id, u);
    
        return new(Units, Props, updated);
    }
    
    public IReadOnlyList<Unit> GetUnis() => Units.Values.ToList();
    public IReadOnlyList<Prop> GetProps() => Props.Values.ToList();
    public IReadOnlyList<Npc> GetNpcs() => Npcs.Values.ToList();
}





