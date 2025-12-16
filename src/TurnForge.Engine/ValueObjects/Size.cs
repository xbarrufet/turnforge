namespace TurnForge.Engine.ValueObjects;

public readonly record struct Size(int Width, int Height)
{
    public static Size Parse(string value)
    {
        var parts = value.Split('x');
        return new Size(
            int.Parse(parts[0]),
            int.Parse(parts[1]));
    }
}