
using System.Collections.Generic;
using System.Linq;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Board;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;

namespace TurnForge.Engine.Core.Fsm.SystemNodes
{
    /// <summary>
    /// Initial node - allows board initialization.
    /// Transitions to BoardReady after board is set up.
    /// </summary>
    public class InitialStateNode : LeafNode
    {
        public InitialStateNode()
        {
            AddAllowedCommand<InitializeBoardCommand>();
        }

        public override bool IsCommandValid(ICommand command, GameState state)
        {
            return command is InitializeBoardCommand;
        }

        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = result.Tags != null && result.Tags.Contains("BoardInitialized");
            return Enumerable.Empty<IFsmApplier>();
        }
    }
}
