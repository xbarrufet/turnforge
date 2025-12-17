using BarelyAlive.Godot.Infrastructure;
using Godot;

namespace BarelyAlive.Godot.controllers;

using Godot;

public partial class RootController : Node
{
    [Export] private Control _gameWorldPanel;
    [Export] private MapPresenter _mapPresenter;
    private GameContext _gameContext;

    public override void _Ready()
    {
        // Intento inicial
        TryFitMap();
        _gameContext = GameContext.Instance;

        // Misión cargada
        _gameContext.MissionContextChanged += OnMissionLoaded;
        _gameWorldPanel.Resized += OnGameWorldPanelResized;
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
