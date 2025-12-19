using Godot;
using System;
using System.Collections.Generic;

public partial class GameContext : Node
{
    public static GameContext Instance { get; private set; }
    Texture2D _mapTexture;
    Dictionary<string, Rect2> _areas = new Dictionary<string, Rect2>();
    public Texture2D MapTexture => _mapTexture;
    public Dictionary<string, Rect2> Areas => _areas;   
    
    private Vector2 _scaleWorldToMap = Vector2.One;
    public Vector2 ScaleWorldToMap => _scaleWorldToMap;
    private string _missionName = string.Empty;
    public string MissionName => _missionName;
    private Vector2 _mapWorldSize = Vector2.One;
    public Vector2 MapWorldSize => _mapWorldSize;
    public bool HasMissionContext => _mapTexture != null && _areas.Count > 0;

    [Signal]
    public delegate void MissionContextChangedEventHandler();

    public override void _Ready()
    {
        Instance = this;
        
        
    }
    
    public void SetMissionContext(string missionName, Vector2 mapWorldSize,Texture2D texture,Dictionary<string, Rect2> areas)
    {
        _mapTexture = texture;
        _mapWorldSize= mapWorldSize;
        _missionName = missionName;
        _areas = areas;
        EmitSignal(SignalName.MissionContextChanged);
    }

    public void SetRealTextureSize(Vector2 realTextureSize)
    {
        _scaleWorldToMap= _calculateScale(realTextureSize, _mapWorldSize);
        GD.Print($"[Game Context] scale world to map defined as {_scaleWorldToMap}");
        
    }
    
    private static Vector2 _calculateScale(Vector2 mapSize, Vector2 areaSize)
    {
        return new Vector2(mapSize.X / areaSize.X, mapSize.Y / areaSize.Y);
    }
    
}
