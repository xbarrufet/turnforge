using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities.Actors.Definitions;

namespace TurnForge.Engine.Commands.GameStart;

public sealed record GameStartCommand(IReadOnlyList<AgentDescriptor> Agents) : ICommand
{

}

