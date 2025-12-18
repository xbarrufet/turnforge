using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Commands.LoadGame;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core;

public sealed class GameEngine(CommandBus commandBus)
{
   
    private readonly CommandBus _commandBus= commandBus;
    
    public void LoadGame(InitializeGameCommand command)
    {
        _commandBus.Send(command);
    }
    
    public void GameStart(GameStartCommand command)
    {
        _commandBus.Send(command);
    }

    private ISpatialModel BuildSpatialModel(
        SpatialDescriptor descriptor)
    {
        return descriptor switch
        {
            DiscreteSpatialDescriptor discrete =>
                BuildDiscreteSpatialModel(discrete),

            ContinuousSpatialDescriptior =>
                throw new NotImplementedException(
                    "Continuous spatial model not implemented"),

            _ => throw new NotSupportedException(
                $"Unsupported spatial definition {descriptor.GetType().Name}")
        };
    }
    private ISpatialModel BuildDiscreteSpatialModel(
        DiscreteSpatialDescriptor def)
    {
        var graph = new MutableTileGraph(
            def.Connections.Select(c => (c.From, c.To))
        );

        return new ConnectedGraphSpatialModel(graph);
    }

}