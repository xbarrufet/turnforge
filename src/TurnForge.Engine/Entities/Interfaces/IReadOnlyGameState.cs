using TurnForge.Engine.Entities.Actors;

namespace TurnForge.Engine.Entities.Interfaces;

public interface IReadOnlyGameState
{
    IReadOnlyCollection<Unit> Units { get; }
    IReadOnlyCollection<Prop> Props { get; }
    IReadOnlyCollection<Npc> Npcs { get; }
    
}