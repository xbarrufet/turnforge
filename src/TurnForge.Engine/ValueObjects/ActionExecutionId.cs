namespace TurnForge.Engine.ValueObjects;

    /// <summary>
    /// Strongly typed identifier for a Workflow execution instance.
    /// Useful for logging, debugging and replay.
    /// </summary>
    public readonly record struct WorkflowExecutionId(Guid Value)
    {
        public static WorkflowExecutionId New()
            => new(Guid.NewGuid());

        public override string ToString() => Value.ToString();
    }