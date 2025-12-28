namespace TurnForge.Engine.Traits.Standard.Checkers;

// Defines the scope of a the check
public interface ICheckScope { };

public record OneOfThem : ICheckScope { };
public record AllOfThem : ICheckScope { };
public record SomeOfThem(int Count) : ICheckScope { };
public record AnyOfThem : OneOfThem;