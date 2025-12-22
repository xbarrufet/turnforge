using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Commands.Game;

public class StartGameCommand : ICommand
{
    public List<AgentDescriptor> Agents { get; }

    public StartGameCommand(List<AgentDescriptor> agents)
    {
        Agents = agents;
    }

    public Type CommandType => typeof(StartGameCommand);
}
