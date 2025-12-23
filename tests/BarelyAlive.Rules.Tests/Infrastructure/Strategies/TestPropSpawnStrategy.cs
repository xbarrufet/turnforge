using TurnForge.Engine.Entities.Actors.Descriptors;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.Core;
using TurnForge.Engine.Entities; // for GameState
using TurnForge.Engine.ValueObjects; // for Position
using BarelyAlive.Rules.Tests.Infrastructure;
using System.Linq;

namespace BarelyAlive.Rules.Tests.Infrastructure.Strategies;

public class TestPropSpawnStrategy : ISpawnStrategy<PropDescriptor>
{
    public IReadOnlyList<PropDescriptor> Process(
        IReadOnlyList<PropDescriptor> descriptors,
        GameState state)
    {
        return descriptors;
    }
}
