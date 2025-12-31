using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

/// <summary>
/// Resolver executed automatically when entering an FSM node.
/// Like Commands but without user intervention, used for setup logic.
/// </summary>
public interface INodeResolver
{
    string Name { get; }
    
    /// <summary>
    /// Execute resolver logic. Returns modified state.
    /// </summary>
    ResolverResult Resolve(ResolverContext context);
}

/// <summary>
/// Context provided to resolvers for execution.
/// </summary>
public record ResolverContext(
    Entities.GameState State,
    IServiceProvider Services
);

/// <summary>
/// Result of resolver execution.
/// </summary>
public record ResolverResult(
    Entities.GameState State,
    IReadOnlyList<IGameEvent> Events
)
{
    public static ResolverResult From(Entities.GameState state) 
        => new(state, Array.Empty<IGameEvent>());
    
    public static ResolverResult From(Entities.GameState state, IEnumerable<IGameEvent> events) 
        => new(state, events.ToList());
}
