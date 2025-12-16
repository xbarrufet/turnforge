using System.Numerics;

namespace TurnForge.Engine.ValueObjects;

public readonly record struct Point(int X, int Y)
{
    public static Point Zero => new(0, 0);

    public Point Translate(Point delta)
        => new(X + delta.X, Y + delta.Y);

    public override string ToString()
        => $"({X}, {Y})";
    
}   