using BarelyAlive.Godot.Adapter;
using BarelyAlive.Godot.Infrastructure;
using Godot;

namespace BarelyAlive.Godot.controllers;

using Godot;

public partial class RootController : Node
{
	[Export] private Control _gameWorldPanel;
	[Export] private MapPresenter _mapPresenter;
	[Export] private SurvivorSelectionController _survivorSelectionController;
	[Export] public BarelyAlive.Godot.Resources.Missions.MissionResource StartingMission;
	private GameSession _gameSession;

	public override void _Ready()
	{
		_gameSession = GameSession.Instance;

		// Subscribe first to ensure we catch signals emitted during LoadMission
		_gameSession.MissionContextChanged += OnMissionLoaded;
		_gameWorldPanel.Resized += OnGameWorldPanelResized;
		
		if (_survivorSelectionController != null)
		{
			_survivorSelectionController.GameStartRequested += OnGameStartRequested;
		}

		TryFitMap();

		if (StartingMission != null)
		{
// 			GD.Print($"[RootController] StartingMission is set: {StartingMission.MissionName}. Calling LoadMission.");
// 			TurnForgeAdapter.Instance.LoadMission(StartingMission);
		}
		else
		{
			GD.PushWarning("[RootController] StartingMission is NULL. Please assign a MissionResource in the Inspector.");
		}
	}
	
	private void OnGameStartRequested(string[] survivorIds)
	{
		GD.Print($"[RootController] Game Start Requested with {survivorIds.Length} survivors.");
		TurnForgeAdapter.Instance.StartGame(survivorIds);
	}

	private void OnMissionLoaded()
	{
		TryFitMap();
	}

	private void TryFitMap()
	{
		if (!_mapPresenter.HasMap)
			return;

		_mapPresenter.FitMapToArea(_gameWorldPanel.Size);
	}

	public void OnGameWorldPanelResized()
	{
		TryFitMap();
	}
}
