namespace TurnForge.Engine.Entities.Actors.Capabilities;

public interface IContainer
{
    IReadOnlyList<string> Items { get; }
}