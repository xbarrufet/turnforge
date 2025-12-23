using TurnForge.Engine.Entities.Board.Interfaces;

namespace TurnForge.Engine.Entities.Board.Descriptors;

using System.Collections.Generic;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.ValueObjects;


/// <summary>
/// Descriptor estructural de una Zone.
/// Define que existe una zona en el board, con un bound espacial
/// y un conjunto de behaviours (capacidades).
/// 
/// NO contiene lógica.
/// NO contiene reglas.
/// Viaja en InitializeGameCommand.
/// </summary>
public sealed record ZoneDescriptor
(
    ZoneId Id,
    IZoneBound Bound
);
