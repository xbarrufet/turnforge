
using BarelyAlive.Rules.Adapter.Dto;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Adapter.Mappers;

public static class PositionMapper
{
    public static Vector ToPosition(this PositionDto dto)
        => new(dto.X, dto.Y);
}

