using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Board;


public sealed class Zone
{
    public ZoneId Id => _definition.Id;
    public string Name => _definition.Name;
    private readonly ZoneDefinition _definition;

    public Zone(ZoneDefinition definition)
    {
        _definition = definition;
    }

    public bool Contains(Position position)
        => _definition.Bound.Contains(position);
}
