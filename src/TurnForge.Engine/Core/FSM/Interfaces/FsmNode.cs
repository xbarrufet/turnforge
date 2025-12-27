using System;
using System.Collections.Generic;
using TurnForge.Engine.Commands;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Core.Fsm.Interfaces;

public abstract class FsmNode
{
    public NodeId Id { get; internal init; }
    public string Name { get; init; } = string.Empty;

    // Hierarchy removed (Parent, Children, NextSibling) - Flat List Architecture

    public abstract bool IsCommandAllowed(Type commandType);
    public abstract IReadOnlyList<Type> GetAllowedCommands();

    /// <summary>
    /// Executes the node's main logic.
    /// Returns a set of application decisions/effects and optionally a command to launch.
    /// </summary>
    public virtual NodeExecutionResult Execute(GameState state) 
        => NodeExecutionResult.Empty();

    /// <summary>
    /// Determines if the current node has completed its work and the engine should move to the next node.
    /// Default: true (Pass-through node).
    /// Interactive nodes (Leafs) should return false until a condition is met.
    /// </summary>
    public virtual bool IsCompleted(GameState state) => true;
    
    /// <summary>
    /// Called when a command is executed while this node is active.
    /// Used by interactive nodes to react to external input logic (legacy support or specific logic).
    /// </summary>
    public virtual NodeExecutionResult OnCommandExecuted(ICommand command, CommandResult result)
        => NodeExecutionResult.Empty();
    /// <summary>
    /// Checks if the game should end while in this node.
    /// Default: false.
    /// </summary>
    public virtual bool IsGameOver(GameState state) => false;
}