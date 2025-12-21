using System;
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.LoadGame.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers.Effects;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.Entities.Appliers.Results.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Decisions;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Entities.Components;
using TurnForge.Engine.Entities.Decisions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.Entities.Factories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Spatial.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Appliers;

public sealed class BoardApplier(IGameEntityFactory<GameBoard> factory) : IBuildApplier<BoardDecision, GameBoard>
{
    public ApplierResponse Apply(BoardDecision decision, GameState state)
    {
        var board = factory.Build(decision.Board);
        var boardDescriptor = decision.Board;
        var zonesCreated = new List<ZoneDefinition>();
        foreach (var zoneDesc in boardDescriptor.Zones)
        {
            ZoneDefinition zoneDefinition = new ZoneDefinition(
                new EntityId(GenerateGuid(zoneDesc.Id.Value)), zoneDesc.Bound);
            var zone = new Zone(
               zoneDefinition,
                new BehaviourComponent(zoneDesc.Behaviours.Cast<BaseBehaviour>()));
            board.AddZone(zone);
            zonesCreated.Add(zoneDefinition);
        }
        return new ApplierResponse(state.WithBoard(board), [new BoardApplierResult(board.Id, zonesCreated)]);
    }

    private static Guid GenerateGuid(string input)
    {
        var hash = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.Default.GetBytes(input));
        return new Guid(hash);
    }
}
