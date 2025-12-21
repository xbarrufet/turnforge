using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Appliers.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class LeafNode : FsmNode
{
    private readonly HashSet<Type> _allowedCommands = new();
    public abstract bool IsCommandValid(ICommand command, GameState state);
    public abstract IEnumerable<IFsmApplier> OnCommandExecuted(ICommand command, CommandResult result, out bool transitionRequested);

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

    public void ClearAllowedCommands()
    {
        _allowedCommands.Clear();
    }

    public void RemoveAllowedCommand<T>() where T : ICommand
    {
        _allowedCommands.Remove(typeof(T));
    }


}

