using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Definitions;
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
    
    public void LoadGame(LoadGameCommand command)
    {
        _commandBus.Send(command);
    }

    private ISpatialModel BuildSpatialModel(
        SpatialDefinition definition)
    {
        return definition switch
        {
            DiscreteSpatialDefinition discrete =>
                BuildDiscreteSpatialModel(discrete),

            ContinuousSpatialDefinition =>
                throw new NotImplementedException(
                    "Continuous spatial model not implemented"),

            _ => throw new NotSupportedException(
                $"Unsupported spatial definition {definition.GetType().Name}")
        };
    }
    private ISpatialModel BuildDiscreteSpatialModel(
        DiscreteSpatialDefinition def)
    {
        var graph = new MutableTileGraph(
            def.Connections.Select(c => (c.From, c.To))
        );

        return new ConnectedGraphSpatialModel(graph);
    }

}