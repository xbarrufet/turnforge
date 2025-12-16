namespace TurnForge.Engine.ValueObjects;

public readonly record struct Rectangle
{
    public readonly Point TopLeft { get; }
    public readonly Point BottomRight { get; }
    public readonly int Width => BottomRight.X - TopLeft.X;
    public readonly int Height => BottomRight.Y - TopLeft.Y;
    
    public Rectangle(Point topLeft, Point bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }
    
    public Rectangle(Point topLeft, int width, int height)
    {
        TopLeft = topLeft;
        BottomRight = new Point(topLeft.X + width, topLeft.Y + height);
    }
    
    public Rectangle(int x, int y, int width, int height)
    {
        var topLeft = new Point(x, y);
        TopLeft = topLeft;
        BottomRight = new Point(topLeft.X + width, topLeft.Y + height);
    }

    
    public bool Contains(Position position, bool includeBorders = true)
    {
        return _containsXY(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y, position.X, position.Y, includeBorders);
    }

    public bool Contains(Point point, bool includeBorders = true)
    {
        return _containsXY(TopLeft.X, TopLeft.Y, BottomRight.X, BottomRight.Y, point.X, point.Y, includeBorders);
    }
    
    private bool _containsXY(int topLeftX, int topLeftY,int bottomRightX, int bottomRightY, int pointX, int pointY,bool includeBorders = true)
    {
        if (!includeBorders)
        {
            return pointX > topLeftX && pointX < bottomRightX &&
                   pointY > topLeftY && pointY < bottomRightY;
        }
        return pointX >= topLeftX && pointX<= bottomRightX &&
               pointY >= topLeftY && pointY <= bottomRightY;
    }
    
    
    
}