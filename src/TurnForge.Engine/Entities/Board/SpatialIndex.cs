using System.Collections.Generic;
using TurnForge.Engine.Domain.Board.Spatial.Interfaces;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board;

public class SpatialIndex : ISpatialIndex
{
    private readonly Dictionary<EntityId, IBoardPosition> _entityPositions = new();
    private readonly Dictionary<IBoardPosition, HashSet<EntityId>> _positionEntities = new();

    public SpatialIndex() { }

    private SpatialIndex(
        Dictionary<EntityId, IBoardPosition> entityPositions,
        Dictionary<IBoardPosition, HashSet<EntityId>> positionEntities)
    {
        _entityPositions = entityPositions;
        _positionEntities = positionEntities;
    }

    public ISpatialIndex Clone()
    {
        var newEntityPositions = new Dictionary<EntityId, IBoardPosition>(_entityPositions);
        var newPositionEntities = new Dictionary<IBoardPosition, HashSet<EntityId>>();

        foreach (var kvp in _positionEntities)
        {
            newPositionEntities[kvp.Key] = new HashSet<EntityId>(kvp.Value);
        }

        return new SpatialIndex(newEntityPositions, newPositionEntities);
    }

    public void Register(EntityId entityId, IBoardPosition position)
    {
        if (_entityPositions.ContainsKey(entityId))
        {
            Unregister(entityId);
        }

        _entityPositions[entityId] = position;
        AddToPositionIndex(entityId, position);
    }

    public void Update(EntityId entityId, IBoardPosition newPosition)
    {
        if (_entityPositions.TryGetValue(entityId, out var oldPosition))
        {
            // Optimization: if position hasn't changed (value equality), do nothing
            if (oldPosition.Equals(newPosition)) return;

            RemoveFromPositionIndex(entityId, oldPosition);
        }

        _entityPositions[entityId] = newPosition;
        AddToPositionIndex(entityId, newPosition);
    }

    public void Unregister(EntityId entityId)
    {
        if (_entityPositions.TryGetValue(entityId, out var position))
        {
            RemoveFromPositionIndex(entityId, position);
            _entityPositions.Remove(entityId);
        }
    }

    public IReadOnlyCollection<EntityId> QueryAt(IBoardPosition position)
    {
        if (_positionEntities.TryGetValue(position, out var entities))
        {
            return entities;
        }
        return Array.Empty<EntityId>();
    }

    private void AddToPositionIndex(EntityId entityId, IBoardPosition position)
    {
        if (!_positionEntities.TryGetValue(position, out var set))
        {
            set = new HashSet<EntityId>();
            _positionEntities[position] = set;
        }
        set.Add(entityId);
    }

    private void RemoveFromPositionIndex(EntityId entityId, IBoardPosition position)
    {
        if (_positionEntities.TryGetValue(position, out var set))
        {
            set.Remove(entityId);
            if (set.Count == 0)
            {
                _positionEntities.Remove(position);
            }
        }
    }

    public IBoardPosition? GetEntityPosition(EntityId entityId)
    {
        if (_entityPositions.TryGetValue(entityId, out var position))
        {
            return position;
        }
        return null;
    }
}