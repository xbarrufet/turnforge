using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Items;

public abstract class Item(ItemId id)
{
    public ItemId Id { get;  } = id;
}