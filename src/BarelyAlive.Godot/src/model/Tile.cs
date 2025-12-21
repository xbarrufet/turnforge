using Godot;

namespace BarelyAlive.Godot.Model;

public partial class Tile(string id, Rect2 rect) : GodotObject
{
    public Rect2 Rect = rect;
    public string Id = id;
}
