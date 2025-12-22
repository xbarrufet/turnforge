using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;

namespace BarelyAlive.Godot.Model;

public partial class MapContext:Node
{
    
    [Signal]
    public delegate void MapContextChangedEventHandler();
    
    string MissionName { get; set; }
    Dictionary<string, Rect2> _areas = new Dictionary<string, Rect2>();
    public Texture2D MapTexture { get; set; }
    public ReadOnlyDictionary<string, Rect2> Areas { get; set; }
    public Vector2 MapSize { get; set; }
    public Vector2 ScaleWorldToMap { get; set; }
    public Vector2 ScaleMapToWorld => new Vector2(1 / ScaleWorldToMap.X, 1/ ScaleWorldToMap.Y);
    public Vector2 MapWorldSize { get; set; }
    public string JsonData { get; set; }
    
    public bool IsMapReady => MapTexture != null && _areas.Count > 0;
    
    public void Set(string resourceJsonData, string missionName, Vector2 mapWorldSize,Texture2D texture,Dictionary<string, Rect2> areas)
    {
        GD.Print("[MapContext] Set called. Updating data...");
        JsonData = resourceJsonData;
        MapTexture = texture;
        MapWorldSize= mapWorldSize;
        MissionName = missionName;
        _areas = areas;
        GD.Print($"[MapContext] Emitting Signal MapContextChanged... (Texture valid: {MapTexture != null})");
        EmitSignal(SignalName.MapContextChanged);
    }
    
    public void SetRealTextureSize(Vector2 realTextureSize)
    {
        ScaleWorldToMap= _calculateScale(realTextureSize, MapWorldSize);
        GD.Print($"[Map Context] scale world to map defined as {ScaleWorldToMap}");
    }
    private static Vector2 _calculateScale(Vector2 mapSize, Vector2 areaSize)
    {
        return new Vector2(mapSize.X / areaSize.X, mapSize.Y / areaSize.Y);
    }

    
}