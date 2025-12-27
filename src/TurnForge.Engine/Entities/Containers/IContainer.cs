namespace TurnForge.Engine.Definitions.Actors.Capabilities;

public interface IContainer
{
    IReadOnlyList<string> Items { get; }
}