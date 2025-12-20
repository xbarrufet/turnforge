
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Infrastructure.Appliers.Interfaces;

namespace TurnForge.Engine.Tests.Infrastructure.Registration
{
    internal class BattleRound : BranchNode { } // Root
    internal class MovementPhase : BranchNode { } // Child of Root

    internal class ShootingPhase : LeafNode
    {
        public override bool IsCommandValid(ICommand command, TurnForge.Engine.Entities.GameState state) => true;
        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = false;
            return Enumerable.Empty<IFsmApplier>();
        }
    }

    internal class NormalMove : LeafNode
    {
        public override bool IsCommandValid(ICommand command, TurnForge.Engine.Entities.GameState state) => true;
        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = false;
            return Enumerable.Empty<IFsmApplier>();
        }
    }

    internal class Reinforcements : LeafNode
    {
        public override bool IsCommandValid(ICommand command, TurnForge.Engine.Entities.GameState state) => true;
        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = false;
            return Enumerable.Empty<IFsmApplier>();
        }
    }
}
