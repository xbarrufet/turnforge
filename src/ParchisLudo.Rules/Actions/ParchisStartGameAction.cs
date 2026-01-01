using TurnForge.Engine.Core.Action;
using TurnForge.Engine.Core.Action.Builders;
using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Action.Nodes;
using TurnForge.Engine.Definitions.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Definitions; // For DiscretBoardDefinition
using TurnForge.Engine.Entities.Board; // For ConnectionEntity
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.Entities.Overlay.Operations;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Components;
using TurnForge.Engine.Entities;
using TurnForge.Engine.APIs.Interfaces;
using TurnForge.Engine.Entities.Actors; // ADDED
using TurnForge.Engine.Definitions.Actors;
using Parchis.Rules.Factory; // For legacy reuse or migration

namespace Parchis.Rules.Actions;

public static class ParchisStartGameActionFactory
{
    public const string ActionId = "parchis_game_start";
    
    public static IAction Create()
    {
        var startNode = new ParchisStartGameNode();
        return ActionBuilder.Create(ActionId)
            .AddNode(startNode)
            .Build();
    }
}

public class ParchisStartGameNode : LinkableNode
{
    public override NodeId Id => new("Parchis_Initialize");
    
    public override ActionStepResult Execute(ActionContext context)
    {
        // 1. Retrieve Services injected by TurnForge/Runtime
        if (!context.TryGet<IGameCatalogApi>("System.GameCatalogApi", out var catalog))
            return ActionStepResult.Fail("IGameCatalogApi not found in context");
            
        if (!context.TryGet<IBoardFactory>("System.BoardFactory", out var boardFactory))
            return ActionStepResult.Fail("IBoardFactory not found in context");

        // 2. Retrieve Parameters
        if (!context.TryGet<string>("BoardId", out var boardId))
            return ActionStepResult.Fail("Missing BoardId parameter");
            
        List<string>? playerIdsList = null;
        if (context.TryGet<Dictionary<string, string>>("Players", out var playersData))
        {
             // If dictionary passed, extract keys
             playerIdsList = playersData.Keys.ToList();
        }
        else if (context.TryGet<List<string>>("PlayerIds", out var list))
        {
             playerIdsList = list;
        }

        if (playerIdsList == null)
             return ActionStepResult.Fail("Missing Players/PlayerIds parameter");
             
        var players = playerIdsList.Select(id => new PlayerId(id)).ToArray();

        // 3. Create Board
        if (!catalog.TryGetDefinition<DiscretBoardDefinition>(boardId, out var boardDef) || boardDef == null)
            return ActionStepResult.Fail($"Board Definition '{boardId}' not found");

        var board = boardFactory.CreateGameBoard(boardDef);
        context.Overlay.Record(new SetBoardOperation(board));
        
        // 4. Spawn Entities
        var ops = CreateInitialEntities(players);
        foreach(var op in ops)
        {
            context.Overlay.Record(op);
        }
        
        // 5. Set Turn Order
        var turnOrder = TurnOrderState.Create(players);
        context.Overlay.Record(new SetTurnOrderOperation(turnOrder));
        
        return ActionStepResult.Success();
    }
    
    private List<SpawnEntityOperation> CreateInitialEntities(PlayerId[] players)
    {
        var ops = new List<SpawnEntityOperation>();
        
        // Connections
        foreach (var desc in Rules.Factory.ParchisMissionFactory.CreateConnectionDescriptors())
        {
            var connId = desc.DefinitionId ?? $"conn_{desc.From.Value}_{desc.To.Value}";
            var ent = new ConnectionEntity(EntityId.New(), connId, connId, desc.Category);
            var pos = ConnectionPosition.Between(desc.From.Value, desc.To.Value);
            ent.AddComponent(new BasePositionComponent { CurrentPosition = pos });
            if (!string.IsNullOrEmpty(desc.RestrictedToTeam))
                ent.AddComponent(new TeamComponent(new TurnForge.Engine.Traits.Standard.TeamTrait(desc.RestrictedToTeam, "System", null)));
            ops.Add(new SpawnEntityOperation(ent.Id, ent, pos));
        }
        
         // Pawns
        var colors = new[] { "red", "blue", "green", "yellow" };
        
        for(int pIdx = 0; pIdx < players.Length && pIdx < 4; pIdx++)
        {
            var color = colors[pIdx];
            var playerId = players[pIdx];
            
            for (int i = 0; i < 4; i++)
            {
                var pawn = new Agent(EntityId.New(), $"pawn_{color.ToLower()}_{i}", $"{color} Pawn {i}", "Pawn");
                var spawnPos = new TilePosition(new TileId($"spawn_{color.ToLower()}"));
                pawn.SetPositionComponent(new BasePositionComponent { CurrentPosition = spawnPos });
                pawn.ReplaceComponent(new TeamComponent(new TurnForge.Engine.Traits.Standard.TeamTrait(color, "Player", playerId)));
                pawn.ControllerId = playerId.Value;
                ops.Add(new SpawnEntityOperation(pawn.Id, pawn, spawnPos));
            }
        }
        
        return ops;
    }
}
