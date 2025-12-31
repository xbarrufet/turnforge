using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Descriptors;

namespace TurnForge.Engine.Infrastructure.Factories.Interfaces;

public interface IPlayerFactory
{
    Player BuildPlayer(PlayerDescriptor descriptor);
}
