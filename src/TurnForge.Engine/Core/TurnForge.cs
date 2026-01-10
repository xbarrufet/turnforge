using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

public sealed class TurnForge
{
    public IGameEngine Runtime { get; }
    public IGameCatalogApi GameCatalog { get; }

    internal TurnForge(
        IGameEngine runtime,
        IGameCatalogApi gameCatalog)
    {
        Runtime = runtime;
        GameCatalog = gameCatalog;
    }

    // Facade Methods for convenience
    public ActionTransaction ExecuteAction(ActionId actionId, Dictionary<string, object>? parameters = null)
    {
        parameters ??= new Dictionary<string, object>();
        parameters["System.GameCatalogApi"] = GameCatalog;
        return Runtime.ExecuteAction(actionId, parameters);
    }

    /*public CommandTransaction ExecuteCommand(ICommand command)
        => Runtime.ExecuteCommand(command);
*/
    
    public GameStatus GetStatus() => Runtime.GetStatus();

    public global::TurnForge.Engine.Entities.GameState CurrentState => Runtime.CurrentState;

    public void ResetGame() => Runtime.ResetGame();
}
