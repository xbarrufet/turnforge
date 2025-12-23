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
            typeof(TurnForge.Engine.Commands.Spawn.SpawnAgentsCommand)
         }; 

        public override bool IsCommandAllowed(Type commandType) => true; // Allow interaction
        
        // Stays active until logic determines end of phase (e.g. all AP used)
        public override bool IsCompleted(GameState state) => false; 
    }

    public class ZombiesActivationNode : BaseGameNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
        public override bool IsCommandAllowed(Type commandType) => false; // Automated?
        
        // Placeholder: immediate pass or wait for logic
        public override bool IsCompleted(GameState state) => false; 
    }

    public class ZombiesSpawnNode : BaseGameNode
    {
        public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
        public override bool IsCommandAllowed(Type commandType) => false;
        
        public override bool IsCompleted(GameState state) => false; 
    }
}
