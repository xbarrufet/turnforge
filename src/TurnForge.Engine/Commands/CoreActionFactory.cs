using TurnForge.Engine.Commands.StartGame.Action;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Spawn;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Commands;

/// <summary>
/// Factory for creating core engine actions.
/// </summary>
public class GlobalActionFactory : IActionFactory
{
    private readonly IBoardFactory _boardFactory;
    private readonly ISpawnService _spawnService;
    private readonly IActionFactory _customActionFactory;

    public GlobalActionFactory(IBoardFactory boardFactory, ISpawnService spawnService, IActionFactory customActionFactory)
    {
        _boardFactory = boardFactory;
        _spawnService = spawnService;
        _customActionFactory = customActionFactory;
    }

    public IAction BuildAction(ActionId actionId)
    {
        // 1. Check Core Actions
        if (actionId == CoreActions.StartGameActionId)
        {
            return StartGameAction.Create(_boardFactory, _spawnService);
        }

        // 2. Delegate to Custom Factory
        if (_customActionFactory.GetRegisteredActionIds().Contains(actionId))
        {
            return _customActionFactory.BuildAction(actionId);
        }

        throw new ArgumentOutOfRangeException(nameof(actionId), $"No action found for id {actionId.Value}");
    }

    public IReadOnlyList<ActionId> GetRegisteredActionIds()
    {
        var coreIds = new[] { CoreActions.StartGameActionId };
        return coreIds.Concat(_customActionFactory.GetRegisteredActionIds()).ToList();
    }
}