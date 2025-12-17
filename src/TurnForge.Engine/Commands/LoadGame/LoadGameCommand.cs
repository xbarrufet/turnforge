using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Commands.LoadGame;

public sealed class LoadGameCommand(
    SpatialDescriptor spatial,
    IReadOnlyList<PropDescriptor> props
    
) : ICommand
{
    public SpatialDescriptor Spatial { get; } = spatial;
    public IReadOnlyList<PropDescriptor> Props { get; } = props;
    
}