using Godot;
using System;
using System.Collections.Generic;
using BarelyAlive.Godot.Model;

public partial class GameSession : Node
{
    public static GameSession Instance { get; private set; }
    
    [Export] public ViewModel  ViewModel { get; set; }
    [Export] public MapContext MapContext { get; set; }
    
    [Signal]
    public delegate void MissionContextChangedEventHandler();

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        if (MapContext == null) MapContext = GetNodeOrNull<MapContext>("MapContext");
        if (ViewModel == null) ViewModel = GetNodeOrNull<ViewModel>("ViewModel");
        
        if (MapContext == null) GD.PrintErr("[GameSession] MapContext not found!");
        if (ViewModel == null) GD.PrintErr("[GameSession] ViewModel not found!");
    }
}
