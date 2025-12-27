using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Core.Fsm;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class LeafNode : FsmNode
{
    private readonly HashSet<Type> _allowedCommands = new();

    public void AddAllowedCommand<T>() where T : ICommand
    {
        _allowedCommands.Add(typeof(T));
    }

    public override bool IsCommandAllowed(Type commandType)
    {
        return _allowedCommands.Contains(commandType);
    }

    public override IReadOnlyList<Type> GetAllowedCommands()
    {
        return _allowedCommands.ToList();
    }
    
    // By default, LeafNodes serve as stops, so Execute does nothing unless overridden.
    public override NodeExecutionResult Execute(GameState state) => NodeExecutionResult.Empty();

    // LeafNode is NEVER completed automatically. It relies on state changes triggered by commands.
    // Subclasses MUST override if they have an exit condition. Or they stay forever?
    // Actually, Interactive Node waits for input.
    // We default to false.
    public override bool IsCompleted(GameState state) => false;
}
