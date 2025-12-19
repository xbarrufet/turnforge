using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

public sealed class GameEngineRuntime(CommandBus commandBus): IGameEngine
{
   
    private readonly CommandBus _commandBus= commandBus;
    
    public CommandResult InitializeGame(InitializeGameCommand command)
    {
        return _commandBus.Send(command);
    }
    
    public CommandResult StartGame(GameStartCommand command)
    {
       return _commandBus.Send(command);
    }


    public CommandResult Send(ICommand command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        return command switch
        {
            InitializeGameCommand initCommand => InitializeGame(initCommand),
            GameStartCommand startCommand => StartGame(startCommand),
            // add other concrete command types here:
            // SomeOtherCommand c => _commandBus.Send(c),

            // fallback to the generic dispatch
            _ =>  CommandResult.Fail("Command type not supported by runtime.")
        };
    }

}