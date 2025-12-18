using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Commands.GameStart;

public sealed record GameStartCommand(IReadOnlyList<UnitDescriptor> PlayerUnits):ICommand
{
    
}

