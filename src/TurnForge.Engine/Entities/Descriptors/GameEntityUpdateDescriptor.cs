using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Descriptors;

public class GameEntityUpdateDescriptor(EntityId entityId):IGameEntityUpdateDescriptor
{
    public EntityId Id { get; set;} = entityId;
}
