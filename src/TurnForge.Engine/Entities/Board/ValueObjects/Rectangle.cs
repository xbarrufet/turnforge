namespace TurnForge.Engine.ValueObjects;

public readonly record struct Rectangle
{
    public readonly Vector TopLeft { get; }
    public readonly Vector BottomRight { get; }
    public readonly int Width => BottomRight.X - TopLeft.X;
    public readonly int Height => BottomRight.Y - TopLeft.Y;

    public Rectangle(Vector topLeft, Vector bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public Rectangle(Vector topLeft, int width, int height)
    {
        TopLeft = topLeft;
        BottomRight = new Vector(topLeft.X + width, topLeft.Y + height);
    }

    public Rectangle(int x, int y, int width, int height)
    {
        var topLeft = new Vector(x, y);
        TopLeft = topLeft;
        BottomRight = new Vector(topLeft.X + width, topLeft.Y + height);
    }


    public bool Contains(Vector position, bool includeBorders = true)
    {
        return ContainsXY(TopLeft, BottomRight, position, includeBorders);
    }


    private static bool ContainsXY(Vector topLeft, Vector bottomRight, Vector point, bool includeBorders = true)
    {
        if (!includeBorders)
        {
            return point.X > topLeft.X && point.X < bottomRight.X &&
                   point.Y > topLeft.Y && point.Y < bottomRight.Y;
        }
        return point.X >= topLeft.X && point.X <= bottomRight.X &&
               point.Y >= topLeft.Y && point.Y <= bottomRight.Y;
    }



}