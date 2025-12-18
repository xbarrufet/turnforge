using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Board.Descriptors;

namespace TurnForge.Engine.Commands.LoadGame;

public sealed class InitializeGameCommand(
    SpatialDescriptor spatial,
    IReadOnlyList<ZoneDescriptor> zones,
    IReadOnlyList<PropDescriptor> startingProps
    
) : ICommand
{
    public SpatialDescriptor Spatial { get; } = spatial;
    public IReadOnlyList<PropDescriptor> StartingProps { get; } = startingProps;
    public IReadOnlyList<ZoneDescriptor> Zones { get; } = zones;
    
}