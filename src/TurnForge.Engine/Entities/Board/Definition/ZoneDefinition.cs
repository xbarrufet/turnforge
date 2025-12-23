using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

public readonly record struct ZoneDefinition(
    ZoneId Id,
    string Name,
    IZoneBound Bound);
