
using System.Collections.Generic;
using TurnForge.Engine.Commands.Interfaces;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Commands.Game;

public sealed record SpawnAgentsCommand(IReadOnlyList<AgentDescriptor> PlayerAgents) : ICommand;
