
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.StartGame.Action;
using TurnForge.Engine.Commands.ValueObjects;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;

namespace TurnForge.Engine.Commands.StartGame;

using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.ValueObjects;

public sealed class StartGameCommand : ICommand
{
    public IAction Action => _workflow;
    private readonly IAction _workflow;

    public StartGameCommand(IBoardFactory boardFactory, IEntityApplier entityApplier)
    {
        _workflow = StartGameAction.Create(boardFactory, entityApplier);
    }

    public CommandType CommandType => CommandType.StartGame;
}