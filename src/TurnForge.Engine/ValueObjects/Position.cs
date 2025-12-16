namespace TurnForge.Engine.ValueObjects;

public readonly record struct Position(int X, int Y)
{
    public static Position Zero => new(0, 0);

    public Position MoveTo(Position newPosition)
        => new(newPosition.X, newPosition.Y);

    public override string ToString()
        => $"({X}, {Y})";
    
} 
