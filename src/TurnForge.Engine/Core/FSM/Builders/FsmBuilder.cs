using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Builders;

/// <summary>
/// Fluent builder for creating FSM phase sequences.
/// </summary>
public class FsmBuilder
{
    private readonly List<FsmNode> _phases = new();
    private int _nodeCounter = 0;
    
    private FsmBuilder() { }
    
    /// <summary>
    /// Start building an FSM.
    /// </summary>
    public static FsmBuilder Create()
    {
        return new FsmBuilder();
    }
    
    /// <summary>
    /// Add a phase with allowed command types.
    /// </summary>
    public FsmBuilder Phase<TPhase>(params Type[] allowedCommands) where TPhase : FsmNode, new()
    {
        var phase = new TPhase();
        // Note: FsmNode.Id is internal init, so handled by FsmController
        _phases.Add(phase);
        return this;
    }
    
    /// <summary>
    /// Add a simple phase with specified allowed commands and completion condition.
    /// </summary>
    public FsmBuilder Phase(
        string name,
        Func<GameState, bool> isCompleted,
        params Type[] allowedCommands)
    {
        var phase = new SimpleFsmPhase(
            name,
            isCompleted,
            allowedCommands);
        _phases.Add(phase);
        return this;
    }
    
    /// <summary>
    /// Add a pass-through phase (always completes immediately).
    /// </summary>
    public FsmBuilder PassThroughPhase<TPhase>() where TPhase : FsmNode, new()
    {
        var phase = new TPhase();
        _phases.Add(phase);
        return this;
    }
    
    /// <summary>
    /// Build the FSM sequence.
    /// </summary>
    public IEnumerable<FsmNode> Build()
    {
        return _phases.AsReadOnly();
    }
    
    /// <summary>
    /// Build and create an FsmController.
    /// </summary>
    public FsmController BuildController()
    {
        return new FsmController(_phases);
    }
}

/// <summary>
/// Simple FSM phase created by the builder.
/// </summary>
internal sealed class SimpleFsmPhase : FsmNode
{
    private readonly string _name;
    private readonly Func<GameState, bool> _isCompleted;
    private readonly Type[] _allowedCommands;
    
    public SimpleFsmPhase(
        string name,
        Func<GameState, bool> isCompleted,
        Type[] allowedCommands)
    {
        _name = name;
        _isCompleted = isCompleted;
        _allowedCommands = allowedCommands;
        Name = name;
    }
    
    public override bool IsCommandAllowed(Type commandType)
        => _allowedCommands.Contains(commandType);
    
    public override IReadOnlyList<Type> GetAllowedCommands()
        => _allowedCommands;
    
    public override bool IsCompleted(GameState state)
        => _isCompleted(state);
}
