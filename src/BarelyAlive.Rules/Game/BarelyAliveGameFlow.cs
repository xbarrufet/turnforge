using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Core.Fsm;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Infrastructure.Registration;

namespace BarelyAlive.Rules.Game;

public static class BarelyAliveGameFlow
{
    public static FsmController CreateController()
    {
        var builder = new GameFlowBuilder();

        var rootNode = builder.AddRoot<TurnForge.Engine.Core.Fsm.SystemNodes.SystemRootNode>("Root", root =>
        {
            root.Add<GameplayNode>("Gameplay");
        }).Build();

        return new FsmController(rootNode, rootNode.Id);
    }

    public abstract class BaseGameNode : LeafNode
    {
        // Default implementation: Valid if type is in allowed list
        public override bool IsCommandValid(ICommand command, GameState state)
        {
            return GetAllowedCommands().Contains(command.GetType());
        }

        // Default implementation: No specific effects, always request transition if command succeeds
        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = result.Success;
            return [];
        }
    }

    public class GameplayNode : BaseGameNode
    {
         public override IReadOnlyList<System.Type> GetAllowedCommands() => []; 

         // Override IsCommandValid to allow all for now, since we haven't listed all commands
         public override bool IsCommandValid(ICommand command, GameState state)
         {
             return true; 
         }
         
         // Gameplay usually doesn't transition automatically after every command
         // So we override execution
         public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
         {
             transitionRequested = false; // Stay in gameplay
             return [];
         }
    }

    public class PlayersPhaseNode : BaseGameNode
    {
        public override IReadOnlyList<System.Type> GetAllowedCommands() => [];
    }

    public class ZombiesActivationNode : BaseGameNode
    {
        public override IReadOnlyList<System.Type> GetAllowedCommands() => [];
    }

    public class ZombiesTurn : BaseGameNode
    {
        public override IReadOnlyList<System.Type> GetAllowedCommands() => [];
        public override IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested)
        {
            transitionRequested = false; // Stay in gameplay
            return [];
        }
    }
}
