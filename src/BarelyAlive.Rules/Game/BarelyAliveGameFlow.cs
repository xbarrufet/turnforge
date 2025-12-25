using System;
using System.Collections.Generic;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Registration;
using TurnForge.Engine.ValueObjects;

namespace BarelyAlive.Rules.Game;

public static class BarelyAliveGameFlow
{
    public static FsmController CreateController()
    {
        var builder = new GameFlowBuilder();

        
        // Flattened Sequence:
        // [System Nodes] -> Players Phase -> Zombies Activation -> Zombies Spawn
        
        builder
            .AddNode<PlayersPhaseNode>("Players Phase")
            .AddNode<ZombiesActivationNode>("Zombies Activation")
            .AddNode<ZombiesSpawnNode>("Zombies Spawn");
        
        var sequence = builder.Build();

        return new FsmController(sequence);
    }

    public abstract class BaseGameNode : LeafNode
    {
        // Default base
    }



    public class PlayersPhaseNode : BaseGameNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands() => new[] 
        { 
            typeof(TurnForge.Engine.Commands.Board.InitializeBoardCommand),
            typeof(TurnForge.Engine.Commands.Spawn.SpawnPropsCommand),
            typeof(TurnForge.Engine.Commands.Spawn.SpawnAgentsCommand),
             typeof(TurnForge.Engine.Commands.Move.MoveCommand)
        }; 

        public override bool IsCommandAllowed(Type commandType) => true; 
        
        public override bool IsCompleted(GameState state) 
        {
             // Check if all Survivors have consumed their AP
             // Create query service on the fly (lightweight)
             if (state.Board == null) return false;
             
             // Wait until survivors are present (prevents premature completion during setup)
             var agents = state.GetAgents();
             if (!System.Linq.Enumerable.Any(agents, a => a.Category == "Survivor"))
             {
                 return false; 
             }
             
             var query = new TurnForge.Engine.Services.Queries.GameStateQueryService(state, state.Board);
             return query.IsAllAgentsActionPointsConsumed("Survivor");
        }
        
        public override bool IsGameOver(GameState state) => CheckGameOver(state);
    }

    public class ZombiesActivationNode : BaseGameNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands() => new[]
        {
             typeof(TurnForge.Engine.Commands.Move.MoveCommand)
        };
        public override bool IsCommandAllowed(Type commandType) => true; 
        
        public override bool IsCompleted(GameState state)
        {
             if (state.Board == null) return false;
             
             var query = new TurnForge.Engine.Services.Queries.GameStateQueryService(state, state.Board);
             return query.IsAllAgentsActionPointsConsumed("Zombie");
        }
        
        public override bool IsGameOver(GameState state) => CheckGameOver(state);
    }

    public class ZombiesSpawnNode : BaseGameNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
        public override bool IsCommandAllowed(Type commandType) => false;
        
        public override NodeExecutionResult Execute(GameState state)
        {
            // Increment round counter
            int currentRound = 0;
            if (state.Metadata.TryGetValue("RoundCounter", out var val) && val is int r)
            {
                currentRound = r;
            }
            
            var nextRound = currentRound + 1;
            
            // Use our new UpdateMetadataApplier
            var applier = new TurnForge.Engine.Appliers.Entity.UpdateMetadataApplier("RoundCounter", nextRound);
            
            // Return decision to update state
            return new NodeExecutionResult(new[] { applier });
        }
        
        public override bool IsCompleted(GameState state) => true; 
        
        public override bool IsGameOver(GameState state) => CheckGameOver(state);
    }
    
    private static bool CheckGameOver(GameState state)
    {
        // 1. Agents met condition
        var survivor = state.GetAgents().FirstOrDefault(a => a.Category == "Survivor");
        var zombie = state.GetAgents().FirstOrDefault(a => a.Category == "Zombie");
        
        if (survivor != null && zombie != null)
        {
            if (survivor.PositionComponent.CurrentPosition == zombie.PositionComponent.CurrentPosition)
            {
                return true;
            }
        }
        
        // 2. Round limit condition
        if (state.Metadata.TryGetValue("RoundCounter", out var val) && val is int rounds)
        {
            if (rounds >= 10) return true;
        }
        
        return false;
    }
}
