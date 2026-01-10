using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players;

namespace TurnForge.Engine.Infrastructure.Factories.Interfaces;

public interface IPlayerFactory
{
    Player BuildPlayer(PlayerDescriptor descriptor);
}
