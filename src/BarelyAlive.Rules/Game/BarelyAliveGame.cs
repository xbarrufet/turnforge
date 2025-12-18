using TurnForge.Engine.Core;

namespace BarelyAlive.Rules.Game;

/// <summary>
/// API pública del juego BarelyAlive.
/// Godot interactúa exclusivamente a través de esta clase.
/// </summary>
public sealed class BarelyAliveGame
{
    
    private readonly GameEngine _gameEngine = GameBootstrap.GameEngineBootstrap();
    
}

