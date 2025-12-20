using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Decisions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class BoardApplier : IBuildApplier<GameBoard>
{

    public GameBoard Build(IGameEntityDescriptor<GameBoard> descriptor, IGameEntityFactory<GameBoard> factory)
    {
        var board = factory.Build(descriptor);
        var boardDescriptor = (BoardDescriptor)descriptor;
        foreach (var zoneDesc in boardDescriptor.Zones)
        {
            var zone = new Zone(
                new TurnForge.Engine.ValueObjects.EntityId(GenerateGuid(zoneDesc.Id.Value)),
                zoneDesc.Bound,
                new TurnForge.Engine.Entities.Components.BehaviourComponent(zoneDesc.Behaviours.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>()));
            board.AddZone(zone);
        }
        return board;
    }

    private static Guid GenerateGuid(string input)
    {
        using var provider = System.Security.Cryptography.MD5.Create();
        var hash = provider.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
        return new Guid(hash);
    }
}
