using TurnForge.Engine.Infrastructure;
using TurnForge.Engine.Core;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;
using Parchis.Rules.Entities;
using Parchis.Rules.Board;
using TurnForge.Engine.Infrastructure.Catalog;

namespace Parchis.Rules;

public static class ParchisBootstrap
{
    public static TurnForge.Engine.Core.TurnForge CreateEngine()
    {
        // 1. Setup Infrastructure
        var repository = new TurnForge.Engine.Infrastructure.Persistence.InMemoryGameRepository();
        
        var context = new GameEngineContext(
            gameRepository: repository
        );

        // 2. Build Engine
        var engine = GameEngineFactory.Build(context);

        // 3. Register Definitions
        RegisterParchisDefinitions(engine);

        return engine;
    }

    private static void RegisterParchisDefinitions(TurnForge.Engine.Core.TurnForge engine)
    {
        // Parchís has 4 players
        var colors = new[] 
        { 
            ParchisBoard.PlayerColor.Red, 
            ParchisBoard.PlayerColor.Blue, 
            ParchisBoard.PlayerColor.Green, 
            ParchisBoard.PlayerColor.Yellow 
        };

        foreach (var color in colors)
        {
            var colorName = color.ToString().ToLower();
            var playerId = new PlayerId($"player_{colorName}");
            
            // Register Player Definition
            var playerDef = new ParchisPlayerDefinition(
                definitionId: $"player_{colorName}_def",
                playerId: playerId,
                color: color
            );
            engine.GameCatalog.RegisterDefinition(playerDef);

            // Register Pawn Definition (Agent)
            // Note: Each color has its own pawn definition owned by that player
            var pawnDef = new ParchisPawnDefinition(
                definitionId: $"pawn_{colorName}_def",
                playerId: playerId,
                color: color
            );
            engine.GameCatalog.RegisterDefinition(pawnDef);
        }
    }
}