using System;
using System.Collections.Generic;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Definitions;

namespace TurnForge.Engine.Tests.Infrastructure.Registration
{
    // Formerly BranchNode, now FsmNode acting as structural pass-through
    internal class BattleRound : FsmNode 
    { 
        public override bool IsCommandAllowed(Type type) => false;
        public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
        public override bool IsCompleted(GameState state) => true; 
    }

    internal class MovementPhase : FsmNode 
    { 
        public override bool IsCommandAllowed(Type type) => false;
        public override IReadOnlyList<Type> GetAllowedCommands() => Array.Empty<Type>();
        public override bool IsCompleted(GameState state) => true; 
    }

    internal class ShootingPhase : LeafNode
    {
        // Interactive: waits.
        public override bool IsCompleted(GameState state) => false;
    }

    internal class NormalMove : LeafNode
    {
        public override bool IsCompleted(GameState state) => false;
    }

    internal class Reinforcements : LeafNode
    {
        public override bool IsCompleted(GameState state) => false;
    }
}
