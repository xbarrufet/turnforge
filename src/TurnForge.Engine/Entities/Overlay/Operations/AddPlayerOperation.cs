using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Overlay.Operations;

public sealed class AddPlayerOperation : IGameStateOperation
{
    public Player Player { get; }
    
    public EntityId Target => EntityId.Empty; // Not entity targeted

    public AddPlayerOperation(Player player)
    {
        Player = player;
    }
    
    public void Apply(IGameStateMutator mutator)
    {
        mutator.AddPlayer(Player);
    }
}
