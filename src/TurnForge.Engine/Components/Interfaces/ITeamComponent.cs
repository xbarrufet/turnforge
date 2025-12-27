using TurnForge.Engine.Components.Interfaces;

namespace TurnForge.Engine.Components.Interfaces;

public interface ITeamComponent : IGameEntityComponent
{
    string Team { get; }
    string ControllerId { get; }
}
