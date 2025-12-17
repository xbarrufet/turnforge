using TurnForge.Engine.Commands.Game.Definitions;
using TurnForge.Engine.Commands.Interfaces;

namespace TurnForge.Engine.Commands.Game;

public sealed class LoadGameCommand(
    SpatialDefinition spatial,
    IReadOnlyList<ActorDefinition> actors
) : ICommand
{
    public SpatialDefinition Spatial { get; } = spatial;
    public IReadOnlyList<ActorDefinition> Actors { get; } = actors;
}