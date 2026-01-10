using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Players.interfaces;

public interface IPlayer
{
    PlayerId PlayerId { get; }
    IActionPool ActionPool { get; set; }

    PlayerControllerType PlayerController { get; }
    string Name { get; }
    string Team { get; }
    
}