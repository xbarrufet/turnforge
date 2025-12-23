using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public readonly record struct RectZoneBound(int X, int Y, int Width, int Height) : IZoneBound
{
    public bool Contains(Position pos)
    {
        if (!pos.IsTile)
        {
            return false;
        }
        return (pos.X >= X && pos.X < X + Width
                           && pos.Y >= Y && pos.Y < Y + Height);
    }


}
