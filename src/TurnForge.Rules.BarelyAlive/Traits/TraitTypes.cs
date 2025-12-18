using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Rules.BarelyAlive.Traits;

public class TraitTypes
{
    public sealed record SpawnOrderTrait(int Order) : IActorBehaviour;
    public sealed record FastTrait(int ExtraMovement) : IActorBehaviour;
    
    public sealed record DarkZoneTrait() : IZoneTrait;
    public sealed record IndoorZoneTrait() : IZoneTrait;
    
}