using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Factories;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Commands.Spawn;

/// <summary>
/// Handler for spawning props using the new spawn pipeline.
/// Pipeline: SpawnRequest → Descriptor (preprocessor) → Strategy → Decision
/// The FSM will apply the decisions using the registered SpawnApplier.
/// </summary>
public sealed class SpawnPropsCommandHandler : ICommandHandler<SpawnPropsCommand>
{
    private readonly ISpawnStrategy<PropDescriptor> _strategy;
    private readonly IGameCatalog _catalog;
    private readonly IGameRepository _repository;

    public SpawnPropsCommandHandler(
        ISpawnStrategy<PropDescriptor> strategy,
        IGameCatalog catalog,
        IGameRepository repository)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public CommandResult Handle(SpawnPropsCommand command)
    {
        // Load current game state
        var gameState = _repository.LoadGameState();

        // STEP 1: PREPROCESSOR - Convert SpawnRequests to Descriptors
        var descriptors = BuildDescriptors(command.Requests);

        // STEP 2: STRATEGY - Process descriptors (filter/modify based on business logic)
        var processedDescriptors = _strategy.Process(descriptors, gameState);

        // STEP 3: TO DECISIONS - Wrap descriptors in spawn decisions
        var spawnDecisions = _strategy.ToDecisions(processedDescriptors);

        // Convert to IDecision array for FSM
        var decisions = spawnDecisions.Cast<IDecision>().ToArray();

        // STEP 4: Return decisions to FSM
        // FSM will apply them using PropSpawnApplier at the appropriate time
        // (either immediately or at OnCommandExecutionEnd depending on Decision.Timing)
        return CommandResult.Ok(
            decisions: decisions,
            tags: "PropsSpawned"
        );
    }

    /// <summary>
    /// Preprocessor: Converts SpawnRequests to populated PropDescriptors.
    /// Applies definition data and property overrides automatically.
    /// </summary>
    private List<PropDescriptor> BuildDescriptors(IReadOnlyList<SpawnRequest> requests)
    {
        var descriptors = new List<PropDescriptor>();

        foreach (var request in requests)
        {
            // Get definition from catalog
            var definition = _catalog.GetDefinition<BaseGameEntityDefinition>(request.DefinitionId);
            if (definition == null)
            {
                // Log warning and skip - definition not found
                // TODO: Add logging
                continue;
            }

            // Expand Count (batch spawn)
            for (int i = 0; i < request.Count; i++)
            {
                // Build descriptor with definition + overrides
                var descriptor = DescriptorBuilder.Build<PropDescriptor>(request, definition);
                descriptors.Add(descriptor);
            }
        }

        return descriptors;
    }
}
