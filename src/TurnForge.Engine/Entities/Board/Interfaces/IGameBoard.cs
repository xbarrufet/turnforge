using TurnForge.Engine.Domain.Board.Spatial.Interfaces;
using TurnForge.Engine.Entities.Board.Enums;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Board.Interfaces;

    public interface IGameBoard
    {
    /// <summary>
    /// Identificador del board (útil para escenarios multi-board).
    /// </summary>
    EntityId Id { get; }

        /// <summary>
        /// Tipo de board (discrete, continuous, hybrid…).
        /// </summary>
        BoardKind Kind { get; }

        /// <summary>
        /// Topología del board (reglas de conexión y traversal).
        /// </summary>
        IBoardTopology Topology { get; }

        /// <summary>
        /// Índice espacial para queries eficientes.
        /// </summary>
        ISpatialIndex SpatialIndex { get; }

        /// <summary>
        /// Valida si una posición pertenece al board.
        /// </summary>
        bool IsValidPosition(IBoardPosition position);

        /// <summary>
        /// Evalúa si se puede atravesar de una posición a otra
        /// bajo un contexto determinado (movimiento, LOS, etc.).
        /// </summary>
       /* TraversalResult CanTraverse(
            IBoardPosition from,
            IBoardPosition to,
            TraversalContext context);*/

        /// <summary>
        /// Devuelve todas las entidades localizadas en una posición.
        /// </summary>
        IReadOnlyList<Engine.ValueObjects.EntityId> GetEntitiesAt(IBoardPosition position);

        /// <summary>
        /// Devuelve entidades que intersectan un área.
        /// Útil para AoE, auras, trampas, continuos.
        /// </summary>
        //IReadOnlyList<EntityId> QueryArea(BoardArea area);
        
        IGameBoard Clone();
    }


