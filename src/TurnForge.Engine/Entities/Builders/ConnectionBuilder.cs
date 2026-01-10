using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Services;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Builders;

public class ConnectionBuilder
{
    private readonly ComponentInitializationService _traitService;

    public ConnectionBuilder(ComponentInitializationService traitService)
    {
        _traitService = traitService;
    }

    /// <summary>
    /// Builds a Zone entity from descriptor and definition.
    /// </summary>
    public Connection Build(ConnectionDescriptor descriptor, ConnectionDefinition definition)
    {
        // Build Connection with From, To, ConnectionPosition from descriptor
        var connection = new Connection(
            EntityId.New(),
            descriptor.DefinitionId,
            descriptor.Name,
            definition.Category,
            descriptor.From,
            descriptor.To,
            descriptor.ConnectionPosition
        );

        // 2. Initialize traits from definition
        foreach (var trait in definition.Traits)
        {
            connection.AddTrait((dynamic)trait);
        }

        // 3. Apply trait overrides from descriptor if any
        var overrides = descriptor.DefinitionTraitValues;
        if (overrides != null)
        {
            foreach (var trait in overrides)
            {
                var traitType = trait.GetType();
                var removeMethod = typeof(GameEntity).GetMethod("RemoveTrait");
                removeMethod?.MakeGenericMethod(traitType)?.Invoke(connection, null);
                connection.AddTrait((dynamic)trait);
            }
        }

        // 4. Initialize components from traits
        _traitService.InitializeComponents(connection);

        // 5. Add extra components from descriptor if any
        var components = descriptor.ExtraComponents;
        if (components != null)
        {
            foreach (var component in components)
            {
                connection.AddComponent((dynamic)component);
            }
        }

        return connection;
    }
}