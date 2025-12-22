using BarelyAlive.Godot.Adapter;
using BarelyAlive.Godot.Model;
using Godot;
using System.Linq;
using System.Collections.Generic;

namespace BarelyAlive.Godot.controllers;

public partial class SurvivorSelectionController : Control
{
    [Export] private PackedScene _rowScene;
    [Export] private Container _listContainer;
    [Export] private Button _startGameButton;
    private ViewModel _viewModel;

    public override void _Ready()
    {
        // Ensure we wait for the game session singleton
        _viewModel = GameSession.Instance.ViewModel;
        GD.Print($"[SurvivorSelection] _Ready. ViewModel is: {(_viewModel != null ? _viewModel.GetInstanceId().ToString() : "NULL")}");
        if (_startGameButton != null)
        {
             _startGameButton.Pressed += OnStartGamePressed;
        }
        else
        {
            GD.PushWarning("[SurvivorSelection] StartGameButton not assigned.");
        }
        if (_viewModel != null)
        {
            _viewModel.AvailableSurvivorsChanged += OnSurvivorsAvailableChanged;
        }
        // Try to fetch immediately if engine is ready, or wait for it
        if (TurnForgeAdapter.Instance.IsInitialized)
        {
            FetchSurvivors();
        }
        else
        {
            TurnForgeAdapter.Instance.GameInitialized += OnGameInitialized;
        }
    }

    private void OnGameInitialized(bool success)
    {
        if (success)
        {
             FetchSurvivors();
        }
    }

    private void FetchSurvivors()
    {
        var survivors = TurnForgeAdapter.Instance.GetAvailableSurvivors();
        _viewModel?.SetAvailableSurvivorsInCatalog(survivors);
    }

    [Signal]
    public delegate void GameStartRequestedEventHandler(string[] survivorIds);

    private readonly HashSet<string> _selectedSurvivors = new();

    private void OnStartGamePressed()
    {
        if (_selectedSurvivors.Count == 0)
        {
            return;
        }
        
        EmitSignal(SignalName.GameStartRequested, _selectedSurvivors.ToArray());
    }

    private void OnSurvivorsAvailableChanged()
    {
        if (_listContainer == null || _rowScene == null) 
        {
            GD.PushWarning("SurvivorSelectionController: ListContainer or RowScene not assigned.");
            return;
        }

        foreach (var child in _listContainer.GetChildren())
        {
            child.QueueFree();
        }
        _selectedSurvivors.Clear();

        foreach (var survivor in _viewModel.AvailableSurvivors)
        {
            // Note: SurviorSelectionRow has been renamed to SurvivorSelectionRow
            var row = _rowScene.Instantiate<SurvivorSelectionRow>();
            _listContainer.AddChild(row);
            
            row.Setup(survivor.Name, null);
            row.Pressed += () => OnSurvivorRowPressed(row, survivor.Id);
        }
    }

    private void OnSurvivorRowPressed(SurvivorSelectionRow row, string survivorId)
    {
        row.ToggleSelection();
        if (row.IsSelected)
        {
            _selectedSurvivors.Add(survivorId);
        }
        else
        {
            _selectedSurvivors.Remove(survivorId);
        }
    }
    
    public override void _ExitTree()
    {
        if (TurnForgeAdapter.Instance != null)
             TurnForgeAdapter.Instance.GameInitialized -= OnGameInitialized;
             
        if (_viewModel != null)
            _viewModel.AvailableSurvivorsChanged -= OnSurvivorsAvailableChanged;
    }
}
