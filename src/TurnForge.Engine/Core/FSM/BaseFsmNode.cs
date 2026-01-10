using TurnForge.Engine.Core.Action.Interfaces;
using TurnForge.Engine.Core.Fsm.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm;

/// <summary>
/// Base class for FSM nodes (FSM 2.0 architecture).
/// 
/// Nodes are flat, explicit, and command-driven:
/// - IsCommandAllowed: validates if a command can be executed
/// - IsCompleted: pure function checking GameState
/// - GetNextNode: determines transition target
/// - OnEntry workflows: system workflows run on node entry
/// </summary>
public abstract class BaseFsmNode : IFsmNode
{
    public NodeId Id { get; protected set; }
    public string Name { get; protected set; } = string.Empty;

    private readonly List<IAction> _onEntryActions = new();

    /// <summary>
    /// System workflows that execute automatically on node entry.
    /// </summary>
    public IReadOnlyList<IAction> OnEntryActions => _onEntryActions.AsReadOnly();

    /// <summary>
    /// Checks if a command type is allowed in this node.
    /// </summary>
    public bool IsActionAllowed(ActionId actionId)
    {
        return GetAllowedActions().Contains(actionId);
    }

    public virtual IReadOnlyList<ActionId> GetAllowedActions()
    {
        return new List<ActionId>();
    }



    /// <summary>
    /// Pure function checking if this node is completed based on state.
    /// When true, FSM will transition to GetNextNode.
    /// </summary>
    public abstract bool IsCompleted(GameStateView state);

    /// <summary>
    /// Returns the next node to transition to.
    /// Can contain complex logic based on state.
    /// </summary>
    public abstract BaseFsmNode? GetNextNode(GameStateView state);

    /// <summary>
    /// Fluent method to add OnEntry workflow.
    /// </summary>
    public BaseFsmNode OnEntry(IAction workflow)
    {
        _onEntryActions.Add(workflow);
        return this;
    }

    protected BaseFsmNode(NodeId id, string name)
    {
        Id = id;
        Name = name;
    }

    protected BaseFsmNode(string name) : this(new NodeId(Guid.NewGuid().ToString()), name) { }
}
