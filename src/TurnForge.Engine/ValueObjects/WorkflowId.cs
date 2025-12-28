namespace TurnForge.Engine.ValueObjects;

/// <summary>
/// Strongly typed identifier for a Workflow.
/// </summary>
public readonly record struct WorkflowId(string Value)
    {
        public override string ToString() => Value;
    }