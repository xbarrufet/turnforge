namespace TurnForge.Engine.ValueObjects;

public readonly record struct Vector(int X, int Y)
{
    public static Vector Zero => new(0, 0);

    public Vector MoveTo(Vector newPosition)
        => new(newPosition.X, newPosition.Y);

    public override string ToString()
        => $"({X}, {Y})";
    public static Vector Empty { get; } = new Vector(int.MinValue, int.MinValue);
    public bool IsEmpty() => this.Equals(Empty);
    
}
