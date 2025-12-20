using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public sealed class RectZoneBound : IZoneBound
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public RectZoneBound(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Contains(Position pos)
    {
        if (!pos.IsDiscrete)
        {
            return false;
        }
        return (pos.X >= X && pos.X < X + Width
                           && pos.Y >= Y && pos.Y < Y + Height);
    }
}
