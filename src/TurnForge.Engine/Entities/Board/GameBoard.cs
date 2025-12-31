using TurnForge.Engine.Domain.Board.Spatial.Interfaces;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;
using EntityId = TurnForge.Engine.ValueObjects.EntityId;

namespace TurnForge.Engine.Entities.Board;


    /// <summary>
    /// Implementación base de un board de juego.
    /// Coordina Topology y SpatialIndex para responder queries espaciales.
    /// </summary>
    public sealed class GameBoard : IGameBoard
    {
        public EntityId Id { get; }
        public BoardKind Kind { get; }

        public IBoardTopology Topology { get; }
        public ISpatialIndex SpatialIndex { get; }

        public GameBoard(
            ValueObjects.EntityId id,
            BoardKind kind,
            IBoardTopology topology,
            ISpatialIndex spatialIndex)
        {
            Id = id;
            Kind = kind;
            Topology = topology ?? throw new ArgumentNullException(nameof(topology));
            SpatialIndex = spatialIndex ?? throw new ArgumentNullException(nameof(spatialIndex));
        }

        public GameBoard(GameBoard other, ISpatialIndex? emptyIndex = null)
        {
            Id = other.Id;
            Kind = other.Kind;
            Topology = other.Topology; 
            // Reuse cloned index or create a fresh one if provided
            SpatialIndex = emptyIndex ?? other.SpatialIndex.Clone();
        }

        public IGameBoard Clone() => new GameBoard(this);

        public IGameBoard CloneWithNewIndex() => new GameBoard(this, new SpatialIndex());

        public bool IsValidPosition(IBoardPosition position)
        {
            return Topology.IsValidPosition(position);
        }

    

        public IReadOnlyList<Engine.ValueObjects.EntityId> GetEntitiesAt(IBoardPosition position)
        {
            if (!IsValidPosition(position))
                return Array.Empty<Engine.ValueObjects.EntityId>();

            return [.. SpatialIndex.QueryAt(position)];
        }


 /*   public TraversalResult CanTraverse(
            IBoardPosition from,
            IBoardPosition to,
            TraversalContext context)
        {
            if (!IsValidPosition(from) || !IsValidPosition(to))
                return TraversalResult.Blocked("Position not on this board");

            // 1️⃣ Regla base de topología
            var topologyResult = Topology.CanTraverse(from, to, context);
            if (!topologyResult.IsAllowed)
                return topologyResult;

            // 2️⃣ Conexiones explícitas (puertas, muros, etc.)
            var connections = SpatialIndex.GetConnectionsBetween(from, to);

            foreach (var connectionId in connections)
            {
                // El Board NO conoce entidades.
                // El Workflow / Reaction resolverá esto usando el GameState.
                var result = context.ConnectionResolver.Evaluate(
                    connectionId,
                    from,
                    to,
                    context);

                if (!result.IsAllowed)
                    return result;
            }

            return TraversalResult.Allowed();
        }
        public IReadOnlyList<EntityId> QueryArea(BoardArea area)
        {
            if (area == null || area.BoardId != Id)
                return Array.Empty<EntityId>();

            return SpatialIndex.QueryArea(area);
        }
        */
    }

