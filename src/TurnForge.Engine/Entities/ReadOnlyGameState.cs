using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Interfaces;
using TurnForge.Engine.States;

namespace TurnForge.Engine.Entities;

internal sealed class ReadOnlyGameState : IReadOnlyGameState
{
    private readonly GameState _state;

    public ReadOnlyGameState(GameState state)
    {
        _state = state;
    }

    public IReadOnlyCollection<Unit> Units => _state.Units.Values;
    public IReadOnlyCollection<Prop> Props => _state.Props.Values;
    public IReadOnlyCollection<Hostile> Hostiles => _state.Hostiles.Values;

    public IEnumerable<TProp> GetProps<TProp>() where TProp : Prop
        => _state.Props.Values.OfType<TProp>();

    public IEnumerable<Unit> GetUnits(Func<Unit, bool> predicate)
        => _state.Units.Values.Where(predicate);
    
    public IEnumerable<Hostile> GetHostiles(Func<Hostile, bool> predicate)
        => _state.Hostiles.Values.Where(predicate);
}
