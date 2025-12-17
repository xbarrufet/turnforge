using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.States;

public sealed class GameState
{
    public Dictionary<ActorId, Unit> Units { get; } = new();
    public Dictionary<ActorId, Prop> Props { get; } = new();
    public Dictionary<ActorId, Hostile> Hostiles { get; } = new();
    
    
    public void AddActor(Actor actor)
    {
        switch (actor)
        {
            case Unit unit:
                AddUnit(unit);
                break;
            case Prop prop:
                AddProp(prop);
                break;
            case Hostile hostile:
                AddHostile(hostile);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actor), $"Unsupported actor type: {actor.GetType().Name}");
        }
    }
    public void AddUnit(Unit unit) => Units[unit.Id] = unit;
    public void AddProp(Prop prop) => Props[prop.Id] = prop;
    public void AddHostile(Hostile hostile) => Hostiles[hostile.Id] = hostile;
    
    public void UpdateUnit(Unit unit) => Units[unit.Id] = unit;
    public void UpdateProp(Prop prop) => Props[prop.Id] = prop;
    public void UpdateHostile(Hostile hostile) => Hostiles[hostile.Id] = hostile;
    
    public void RemoveUnit(ActorId id) => Units.Remove(id);
    public void RemoveProp(ActorId id) => Props.Remove(id);
    public void RemoveHostile(ActorId id) => Hostiles.Remove(id);
    
    public bool IsInitialized => Units.Count > 0 || Props.Count > 0 || Hostiles.Count > 0;
    



}