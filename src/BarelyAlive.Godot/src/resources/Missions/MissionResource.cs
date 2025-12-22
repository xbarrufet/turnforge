using System;
using Godot;

namespace BarelyAlive.Godot.Resources.Missions;

[GlobalClass]
public partial class MissionResource : Resource
{
    //Contains the data of the mission
    [Export]
    public string MissionName = "New Mission";
    [Export]
    public string JsonMissionData = "{}";
    [Export]
    public Texture2D Map = null;
    [Export]
    public Vector2 MapSize;
}
