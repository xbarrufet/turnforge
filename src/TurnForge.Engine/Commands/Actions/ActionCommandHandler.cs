using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Components;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Services.Queries;
using TurnForge.Engine.Strategies.Actions;

namespace TurnForge.Engine.Commands.Actions;

/// <summary>
/// Generic handler for action commands.
/// </summary>
/// <typeparam name="TCommand">Type of action command to handle</typeparam>
/// <remarks>
/// Pipeline: Command → Strategy (validates & builds decisions) → Return to FSM
/// FSM will apply decisions using ActionDecisionApplier.
/// 
/// This handler orchestrates:
/// 1. Load game state from repository
/// 2. Create context (state + board + query service)
/// 3. Execute strategy
/// 4. Return decisions or error to FSM
/// </remarks>
public sealed class ActionCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : IActionCommand
{
    private readonly IActionStrategy<TCommand> _strategy;
    private readonly IGameRepository _repository;
    private readonly IGameStateQuery _queryService;
    private readonly TurnForge.Engine.Strategies.Pipelines.InteractionRegistry _interactionRegistry;
    
    public ActionCommandHandler(
        IActionStrategy<TCommand> strategy,
        IGameStateQuery queryService,
        IGameRepository repository,
        TurnForge.Engine.Strategies.Pipelines.InteractionRegistry interactionRegistry)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        _interactionRegistry = interactionRegistry ?? throw new ArgumentNullException(nameof(interactionRegistry));
    }
    
    public CommandResult Handle(TCommand command)
    {
        // 1. Load current game state
        var gameState = _repository.LoadGameState();
        if (gameState == null)
            return CommandResult.Fail("Game state not found");

        //1.5 validem que tingui ActionPoints > 0
        if(command.HasCost)
        {
            var agent = _queryService.GetAgent(command.AgentId);
            if (agent == null)
                return CommandResult.Fail("Agent not found");
            var apComponent = agent.GetComponent<IActionPointsComponent>();
            if (apComponent == null)
                return CommandResult.Fail("Agent does not have ActionPoints component");
            if (apComponent.CurrentActionPoints <= 0)
                return CommandResult.Fail("Agent has no ActionPoints");
        }



        // 2. Create query service and context
        var context = new ActionContext(gameState, gameState.Board!);
        
        // 3. Execute strategy
        var strategyResult = _strategy.Execute(command, context);
        
        // 4. Return result to FSM
        if (strategyResult.IsSuspended)
        {
            if (strategyResult.Interaction == null)
                return CommandResult.Fail("Strategy suspended but returned no interaction request");
                
            // Register context for resumption
            _interactionRegistry.Register(context);
            
            // Ensure request has correct SessionId matching the context
            var request = strategyResult.Interaction with { SessionId = context.SessionId };
            
            return CommandResult.Suspended(request);
        }
        
        if (!strategyResult.IsValid)
        {
            // Return first error (CommandResult.Fail only accepts single string)
            var error = strategyResult.ValidationErrors.Count > 0
                ? strategyResult.ValidationErrors[0]
                : "Action validation failed";
            
            return CommandResult.Fail(error);
        }
        
        // Success - return decisions for FSM to apply
        var decisions = strategyResult.Decisions.Cast<IDecision>().ToArray();
        return CommandResult.Ok(decisions);
    }
}
