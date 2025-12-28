namespace TurnForge.Engine.ValueObjects;

public abstract record ValidationResult
    {
        public sealed record Ok : ValidationResult;
        public sealed record Cancel : ValidationResult;
        public sealed record Redirect(NodeId TargetNode) : ValidationResult;
        public sealed record Suspend : ValidationResult;

        public static readonly ValidationResult OkResult = new Ok();
        public static readonly ValidationResult CancelResult = new Cancel();
    }