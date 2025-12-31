using System.Runtime.InteropServices;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Overlay;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public sealed class GameStateOverlay
{
    private List<IGameStateOperation> _ordered = new();
    private readonly Dictionary<EntityId, EntityOverlayIndex> _byEntity = new();

    public void Record(IGameStateOperation op)
    {
        _ordered.Add(op);

        ref var index = ref CollectionsMarshal.GetValueRefOrAddDefault(
            _byEntity, op.Target, out _);

        index ??= new EntityOverlayIndex();
        index.Add(op);
    }

    public GameState Commit(GameState baseState)
    {
        var builder = new GameStateBuilder(baseState);
        foreach (var op in _ordered)
        {
            op.Apply(builder);
        }
        return builder.Build();
    }

    public bool TryGetEntity(EntityId id, out GameEntity? entity, out bool isDestroyed)
    {
        entity = null;
        isDestroyed = false;

        if (_byEntity.TryGetValue(id, out var index))
        {
            if (index.Spawn != null)
            {
                entity = index.Spawn.NewEntity;
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Try to get the pending position for an entity.
    /// Returns true if there's a pending move operation.
    /// </summary>
    public bool TryGetPosition(EntityId id, out IBoardPosition? position)
    {
        position = null;
        
        if (_byEntity.TryGetValue(id, out var index) && index.LatestMove != null)
        {
            position = index.LatestMove.NewPosition;
            return true;
        }
        return false;
    }
}


