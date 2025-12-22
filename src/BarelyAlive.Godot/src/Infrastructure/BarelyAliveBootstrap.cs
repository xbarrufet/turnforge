using BarelyAlive.Godot.Adapter;
using BarelyAlive.Godot.controllers;
using Godot;

namespace BarelyAlive.Godot.Infrastructure;

public partial class BarelyAliveBootstrap:Node
{
	
	[Export]
	public string AutoloadMissionPath { get; set; } = 
		"res://src/resources/Missions/mission01.tres";
	
	public override void _Ready()
	{
	    GD.Print("[Bootstrap] _Ready started.");
	   // var gameContext = new GameContext();
	   //AddChild(gameContext);
		TurnForgeAdapter.Instance.Bootstrap();
		GD.Print("[Bootstrap] TurnForgeAdapter bootstrapped.");
		
		if (!string.IsNullOrEmpty(AutoloadMissionPath))
		{
		    GD.Print($"[Bootstrap] Autoloading mission from: {AutoloadMissionPath}");
			var missionLoader = GetNode<MissionSetUpController>("MissionSetUp");
			if (missionLoader != null)
			{
			    GD.Print("[Bootstrap] MissionSetUpController found. Calling SetUpMission...");
			    missionLoader.SetUpMission(AutoloadMissionPath);
			}
			else
			{
			    GD.PrintErr("[Bootstrap] CRITICAL: MissionSetUpController node not found at /root/Boot/MissionSetUp");
			}
		}
		else
		{
		    GD.Print("[Bootstrap] No AutoloadMissionPath specified.");
		}
		GD.Print("[Bootstrap] _Ready completed.");
	}
	
}
