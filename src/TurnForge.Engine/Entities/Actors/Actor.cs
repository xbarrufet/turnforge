using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors;

public abstract class Actor(
        ActorId id,
        Position position,
        IReadOnlyList<IActorTrait>? traits = null,
        string? customType = "")
    {
        public ActorId Id { get; } = id;
        public Position Position { get; set; } = position;
        public string CustomType { get; protected set; } = customType ?? "";
        public IReadOnlyList<IActorTrait> Traits = traits?? new List<IActorTrait>();
        
   
        public bool HasTrait<TTrait>()
            where TTrait : IActorTrait
            => Traits.Any(t => t is TTrait);

}