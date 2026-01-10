namespace TurnForge.Engine.ValueObjects;

    /// <summary>
    /// Strongly typed identifier for a Action execution instance.
    /// Useful for logging, debugging and replay.
    /// </summary>
    public readonly record struct ActionExecutionId(Guid Value)
    {
        public static ActionExecutionId New()
            => new(Guid.NewGuid());

        public static ActionExecutionId Empty => new(Guid.Empty);

        public override string ToString() => Value.ToString();
    }