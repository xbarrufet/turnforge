using System;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class FsmNode
{
    public NodeId Id { get; internal init; }
    public string Name { get; init; } = string.Empty;

    // Estos son datos, no lógica. El nodo no los usa nunca.
    public FsmNode? NextSibling { get; internal set; }
    public BranchNode? Parent { get; internal set; }

    public abstract bool IsCommandAllowed(Type commandType);
    public abstract IReadOnlyList<Type> GetAllowedCommands();

    // El nodo solo se ocupa de su propia vida
    public virtual IEnumerable<IFsmApplier> OnStart(GameState state) => Enumerable.Empty<IFsmApplier>();
    public virtual IEnumerable<IFsmApplier> OnEnd(GameState state) => Enumerable.Empty<IFsmApplier>();
}